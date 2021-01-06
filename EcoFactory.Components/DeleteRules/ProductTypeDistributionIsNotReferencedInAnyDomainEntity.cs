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
  internal class ProductTypeDistributionIsNotReferencedInAnyDomainEntity : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ProductTypeDistributionIsNotReferencedInAnyDomainEntity([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IProductTypeDistribution;
      if (target == null)
      {
        return;
      }
      var entities = _store.Content.OfType<IModel>()
                            .SelectMany(m => m.Entities)
                            .ToArray();
      foreach (var ws in entities.OfType<IWorkstationBase>())
      {
        if (ws.ProcessingDurations.Contains(target))
        {
          ws.RemoveProcessing(target);
        }

        if (ws.SetupDurations.Contains(target))
        {
          ws.RemoveSetup(target);
        }

        if (ws.BatchSizes.Contains(target))
        {
          ws.RemoveBatchSize(target);
        }
      }

      foreach (var ep in entities.OfType<IEntryPoint>())
      {
        if (ep.ArrivalOccurrences.Contains(target))
        {
          ep.RemoveArrival(target);
        }
        if (ep.BatchSizes.Contains(target))
        {
          ep.RemoveBatchSize(target);
        }
      }
    }
  }
}