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
  internal class ProductTypeSpecificResourceIsNotReferencedInAnyWorkstation : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ProductTypeSpecificResourceIsNotReferencedInAnyWorkstation([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IProductTypeSpecificResource;
      if (target == null)
      {
        return;
      }
      foreach (var ws in _store.Content.OfType<IModel>()
                               .SelectMany(m => m.Entities.OfType<IWorkstationBase>())
                               .ToArray())
      {
        if (ws.ProductTypeSpecificProcessingResources.Contains(target))
        {
          ws.RemoveProcessingResource(target);
          break;
        }
      }
    }
  }
}