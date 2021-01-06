#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ProductTypeAmountIsNotReferencedInAnyDomainEntity : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ProductTypeAmountIsNotReferencedInAnyDomainEntity([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IProductTypeAmount;
      if (target == null)
      {
        return;
      }
      var entities = _store.Content.OfType<IModel>()
                            .SelectMany(m => m.Entities)
                            .ToArray();

      foreach (var storage in entities.OfType<IStorage>())
      {
        if (storage.ProductTypeCapacities.Contains(target))
        {
          storage.RemoveProductTypeCapacity(target);
        }
        if (!storage.ProductTypeCapacities.Any())
        {
          storage.HasCapacityPerProductType = false;
          if (storage.Capacity < 1)
          {
            storage.HasLimitedCapacity = false;
          }
        }
      }

      foreach (var transformationRule in entities.OfType<IAssemblyStation>()
                                                 .SelectMany(assemblyStation => assemblyStation.TransformationRules))
      {
        if (transformationRule.Inputs.Contains(target))
        {
          transformationRule.RemoveInput(target);
        }
        foreach (var transformationRuleOutput in
          transformationRule.Outputs.Where(transformationRuleOutput => transformationRuleOutput.Outputs.Contains(target)))
        {
          transformationRuleOutput.Remove(target);
        }
      }

      foreach (var transformationRule in entities.OfType<IProbabilityAssemblyStation>()
                                                 .SelectMany(assemblyStation => assemblyStation.TransformationRules))
      {
        if (transformationRule.Inputs.Contains(target))
        {
          transformationRule.RemoveInput(target);
        }
        foreach (var transformationRuleOutput in
          transformationRule.Outputs.Where(transformationRuleOutput => transformationRuleOutput.Outputs.Contains(target)))
        {
          transformationRuleOutput.Remove(target);
        }
      }

      foreach (var productTerminationCriteria in _store.Content.OfType<IModel>()
                                                        .SelectMany(m => m.Observers)
                                                        .OfType<IProductTerminationCriteria>()
                                                        .Where(productTerminationCriteria => productTerminationCriteria.ProductAmounts.Contains(target)))
      {
        productTerminationCriteria.Remove(target);
      }
    }
  }
}