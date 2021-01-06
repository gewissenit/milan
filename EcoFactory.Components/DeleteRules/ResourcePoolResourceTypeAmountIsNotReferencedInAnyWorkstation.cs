#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.Resources;

namespace EcoFactory.Components.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ResourcePoolResourceTypeAmountIsNotReferencedInAnyWorkstation : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ResourcePoolResourceTypeAmountIsNotReferencedInAnyWorkstation([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IResourcePoolResourceTypeAmount;
      if (target == null)
      {
        return;
      }
      foreach (var ws in _store.Content.OfType<IModel>()
                               .SelectMany(m => m.Entities.OfType<IWorkstationBase>())
                               .ToArray())
      {
        if (ws.ProcessingResources.Contains(target))
        {
          ws.RemoveProcessingResource(target);
          break;
        }
        if (ws.SetupResources.Contains(target))
        {
          ws.RemoveSetupResource(target);
          break;
        }
      }
    }
  }
}