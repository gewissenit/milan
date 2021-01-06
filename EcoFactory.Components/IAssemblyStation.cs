#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.Resources;

namespace EcoFactory.Components
{
  public interface IAssemblyStation : IWorkstationBase
  {
    IEnumerable<ITransformationRule> TransformationRules { get; }
    void AddTransformationRule(ITransformationRule transformationRule);
    void RemoveTransformationRule(ITransformationRule transformationRule);
    IDictionary<ITransformationRuleOutput, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> TransformationRuleSpecificProcessingResourcesDictionary { get; }
  }
}