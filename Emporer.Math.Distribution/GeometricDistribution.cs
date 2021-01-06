#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Discrete geometric distributed stream of pseudo random numbers of type integer.
  ///   Values produced by this distribution are geometric determined by the probability.
  /// </summary>
  public class GeometricDistribution : Distribution, IGeometricDistribution, IIntDistribution, IRealDistribution
  {
    #region Probability

    private double _Probability;

    /// <summary>
    ///   Gets or sets the probability for this distribution.
    ///   (Number of failures before the first success in a sequence
    ///   of independent Bernoulli trials with probability p of success
    ///   on each trial; number of items inspected before encountering the
    ///   first defective item; number of items in a batch of random size;
    ///   number of items demanded from an inventory.)
    /// </summary>
    /// <value>The probability.</value>
    public double Probability
    {
      get { return _Probability; }
      set
      {
        _Probability = value;
        IsValid = (value >= 0) && (value <= 1);
      }
    }

    #endregion

    #region IsValid

    internal override bool IsValid
    {
      set { base.IsValid = value; }
    }

    #endregion

    #region Sample

    /// <summary>
    ///   Abstract method should return the specific Sample as a integer value when implemented in subclasses.
    /// </summary>
    /// <returns>
    ///   The next geometric distributed Sample
    /// </returns>
    int IIntDistribution.GetSample()
    {
      return Convert.ToInt32((this as IRealDistribution).GetSample());
    }

    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        if ((IsAntithetic))
        {
          return System.Math.Floor(System.Math.Log(1 - RandomGenerator.NextDouble()) / System.Math.Log(1 - Probability));
        }
        return System.Math.Floor(System.Math.Log(RandomGenerator.NextDouble()) / System.Math.Log(1 - Probability));
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (!(Probability >= 0) &&
          (Probability <= 1))
      {
        messages.Append("Please check the Probability of the distribution. It must have a value between 0 and 1.");
      }
      return messages;
    }
  }
}