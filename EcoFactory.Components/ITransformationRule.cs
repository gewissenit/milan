#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using Emporer.Math.Distribution;
using Milan.Simulation;

namespace EcoFactory.Components
{
  public interface ITransformationRule : INotifyPropertyChanged
  {
    int Probability { get; set; }
    IEnumerable<IProductTypeAmount> Inputs { get; }
    IEnumerable<ITransformationRuleOutput> Outputs { get; }
    IEmpiricalIntDistribution Distribution { get; set; }
    void AddInput(IProductTypeAmount productTypeAmount);
    void RemoveInput(IProductTypeAmount productTypeAmount);
    void AddOutput(ITransformationRuleOutput output);
    void RemoveOutput(ITransformationRuleOutput output);
    ITransformationRuleOutput GetSampleOutput();
  }
}