#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation.Observers;

namespace Milan.Simulation.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ProductTypeIsNotReferencedInAnyObserver : IDeleteRule
  {
    [Import]
    private IDeleteManager DeleteManager { get; set; }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IProductType;
      if (target == null)
      {
        return;
      }
      foreach (var statistic in target.Model.Observers.OfType<IProductRelated>()
                                      .Where(o => o.ProductType == target).ToArray())
      {
        DeleteManager.Delete(statistic);
      }
    }
  }
}