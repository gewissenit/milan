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
  internal class ProductTypeIsNotReferencedInAnyProductTypeAmount : IDeleteRule
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
      var model = target.Model;
      foreach (var productTypeCapacity in model.Entities.OfType<IStorage>()
                                               .SelectMany(
                                                           storage =>
                                                           storage.ProductTypeCapacities.Where(
                                                                                               productTypeCapacity =>
                                                                                               productTypeCapacity.ProductType == target)).ToArray())
      {
        DeleteManager.Delete(productTypeCapacity);
      }

      foreach (var productAmount in model.Observers.OfType<IProductTerminationCriteria>()
                                         .SelectMany(
                                                     productTerminationCriteria =>
                                                     productTerminationCriteria.ProductAmounts.Where(pa => pa.ProductType == target)).ToArray())
      {
        DeleteManager.Delete(productAmount);
      }

      foreach (var transformationRule in model.Entities.OfType<IAssemblyStation>()
                                              .SelectMany(assemblyStation => assemblyStation.TransformationRules).ToArray())
      {
        foreach (var input in transformationRule.Inputs.Where(i => i.ProductType == target).ToArray())
        {
          DeleteManager.Delete(input);
        }
        foreach (
          var output in
            transformationRule.Outputs.SelectMany(transformationRuleOutput => transformationRuleOutput.Outputs.Where(o => o.ProductType == target)).ToArray())
        {
          DeleteManager.Delete(output);
        }
      }
      
      foreach (var transformationRule in model.Entities.OfType<IProbabilityAssemblyStation>()
                                              .SelectMany(assemblyStation => assemblyStation.TransformationRules).ToArray())
      {
        foreach (var input in transformationRule.Inputs.Where(i => i.ProductType == target).ToArray())
        {
          DeleteManager.Delete(input);
        }
        foreach (
          var output in
            transformationRule.Outputs.SelectMany(transformationRuleOutput => transformationRuleOutput.Outputs.Where(o => o.ProductType == target)).ToArray())
        {
          DeleteManager.Delete(output);
        }
      }
    }
  }
}