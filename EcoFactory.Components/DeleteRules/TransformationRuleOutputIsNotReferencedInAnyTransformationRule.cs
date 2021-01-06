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
  internal class TransformationRuleOutputIsNotReferencedInAnyTransformationRule : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public TransformationRuleOutputIsNotReferencedInAnyTransformationRule([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as ITransformationRuleOutput;
      if (target == null)
      {
        return;
      }
      var entities = _store.Content.OfType<IModel>()
                            .SelectMany(m => m.Entities)
                            .ToArray();
      foreach (var transformationRule in entities.OfType<IAssemblyStation>()
                                                 .SelectMany(ass => ass.TransformationRules)
                                                 .Where(transformationRule => transformationRule.Outputs.Contains(target)))
      {
        transformationRule.RemoveOutput(target);
      }

      foreach (var transformationRule in entities.OfType<IProbabilityAssemblyStation>()
                                                 .SelectMany(ass => ass.TransformationRules)
                                                 .Where(transformationRule => transformationRule.Outputs.Contains(target)))
      {
        transformationRule.RemoveOutput(target);
      }
    }
  }
}