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
  internal class TransformationRuleIsNotReferencedInAnyAssemblyStation : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public TransformationRuleIsNotReferencedInAnyAssemblyStation([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as ITransformationRule;
      if (target == null)
      {
        return;
      }
      var entities = _store.Content.OfType<IModel>()
                            .SelectMany(m => m.Entities)
                            .ToArray();

      foreach (var assemblyStation in entities.OfType<IAssemblyStation>()
                                              .Where(assemblyStation => assemblyStation.TransformationRules.Contains(target)))
      {
        assemblyStation.RemoveTransformationRule(target);
      }
      foreach (var assemblyStation in entities.OfType<IProbabilityAssemblyStation>()
                                              .Where(assemblyStation => assemblyStation.TransformationRules.Contains(target)))
      {
        assemblyStation.RemoveTransformationRule(target);
      }
    }
  }
}