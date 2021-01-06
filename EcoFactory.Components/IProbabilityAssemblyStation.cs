#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Math.Distribution;

namespace EcoFactory.Components
{
  public interface IProbabilityAssemblyStation : IAssemblyStation
  {
    //IEnumerable<ITransformationRule> TransformationRules { get; }
    IEmpiricalIntDistribution TransformationSelectionDistribution { get; set; }
    //IDictionary<ITransformationRuleOutput, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> TransformationRuleSpecificProcessingResourcesDictionary { get; }
    //void AddTransformationRule(ITransformationRule transformationRule);
    //void RemoveTransformationRule(ITransformationRule transformationRule);
  }
}