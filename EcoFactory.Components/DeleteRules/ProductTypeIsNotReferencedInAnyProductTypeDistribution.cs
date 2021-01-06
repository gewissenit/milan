#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation;

namespace EcoFactory.Components.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ProductTypeIsNotReferencedInAnyProductTypeDistribution : IDeleteRule
  {
    [Import]
    private IDeleteManager DeleteManager
    {
      get;
      set;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IProductType;
      if (target == null)
      {
        return;
      }
      var model = target.Model;

      foreach (var ws in model.Entities.OfType<IWorkstationBase>())
      {
        foreach (
          var productTypeDistribution in ws.ProcessingDurations.Where(productTypeDistribution => productTypeDistribution.ProductType == target).ToArray())
        {
          DeleteManager.Delete(productTypeDistribution);
        }

        foreach (var productTypeDistribution in ws.SetupDurations.Where(productTypeDistribution => productTypeDistribution.ProductType == target).ToArray())
        {
          DeleteManager.Delete(productTypeDistribution);
        }

        foreach (var productTypeDistribution in ws.BatchSizes.Where(productTypeDistribution => productTypeDistribution.ProductType == target).ToArray())
        {
          DeleteManager.Delete(productTypeDistribution);
        }
      }

      foreach (var ep in model.Entities.OfType<IEntryPoint>())
      {
        foreach (
          var productTypeDistribution in ep.ArrivalOccurrences.Where(productTypeDistribution => productTypeDistribution.ProductType == target).ToArray())
        {
          DeleteManager.Delete(productTypeDistribution);
        }
        foreach (var productTypeDistribution in ep.BatchSizes.ToArray()
                                                  .Where(productTypeDistribution => productTypeDistribution.ProductType == target).ToArray())
        {
          DeleteManager.Delete(productTypeDistribution);
        }
      }
    }
  }
}