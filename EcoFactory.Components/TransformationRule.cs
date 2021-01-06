#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.JsonStore;
using Milan.Simulation;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class TransformationRule : DomainEntity, ITransformationRule
  {
    [JsonProperty]
    private readonly IList<IProductTypeAmount> _Inputs = new List<IProductTypeAmount>();

    private IEmpiricalIntDistribution _distribution;

    [JsonProperty]
    private IList<ITransformationRuleOutput> _Outputs = new List<ITransformationRuleOutput>();

    public IEmpiricalIntDistribution Distribution
    {
      get { return _distribution; }
      set
      {
        _distribution = value;
        if (_distribution != null)
        {
          _Outputs = _Outputs.OrderBy(tro => tro.Probability)
                               .ToList();
          var cumulativeFrequency = 0;
          foreach (var transformationRuleOutput in _Outputs)
          {
            cumulativeFrequency += transformationRuleOutput.Probability;
            if (cumulativeFrequency > 100)
            {
              throw new InvalidOperationException(
                "The probabilities of a transformation rule is not well defined. The probabilities should be in sum 100.");
            }
            if (!_distribution.TryAddEntry(_Outputs.IndexOf(transformationRuleOutput), (double) cumulativeFrequency / 100))
            {
              throw new InvalidOperationException(
                "The probabilities of a transformation rule is not well defined. The probabilities should be in sum 100.");
            }
          }
          if (cumulativeFrequency < 100)
          {
            throw new InvalidOperationException(
              "The probabilities of a transformation rule is not well defined. The probabilities should be in sum 100.");
          }
        }
      }
    }

    public IEnumerable<IProductTypeAmount> Inputs
    {
      get { return _Inputs; }
    }

    public IEnumerable<ITransformationRuleOutput> Outputs
    {
      get { return _Outputs; }
    }

    [JsonProperty]
    public int Probability
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    public void AddInput(IProductTypeAmount input)
    {
      if (_Inputs.Any(cm => cm.ProductType == input.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _Inputs.Add(input);
    }


    public void AddOutput(ITransformationRuleOutput output)
    {
      _Outputs.Add(output);
    }
    
    public ITransformationRuleOutput GetSampleOutput()
    {
      var sample = Distribution.GetSample();
      var sampleOutput = _Outputs[sample];
      return sampleOutput;
    }

    public void RemoveInput(IProductTypeAmount input)
    {
      if (!_Inputs.Contains(input))
      {
        throw new InvalidOperationException("The given productTypeDistribution does not exist.");
      }
      _Inputs.Remove(input);
    }

    public void RemoveOutput(ITransformationRuleOutput output)
    {
      if (!_Outputs.Contains(output))
      {
        throw new InvalidOperationException("The given output does not exist.");
      }
      _Outputs.Remove(output);
    }
  }
}