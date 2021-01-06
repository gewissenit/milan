#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Discrete geometric distributed stream of pseudo random numbers of type integer.
  ///   Values produced by this distribution are geometric determined by the probability.
  /// </summary>
  public interface IGeometricDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the probability for this distribution.
    ///   (Number of failures before the first success in a sequence
    ///   of independent Bernoulli trials with probability p of success
    ///   on each trial; number of items inspected before encountering the
    ///   first defective item; number of items in a batch of random size;
    ///   number of items demanded from an inventory.)
    /// </summary>
    /// <value>The probability have to be a value between 0 and 1.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Probability")]
    [Category("Parameters")]
    [Description(
      "Gets or sets the probability for this distribution.(Number of failures before the first success in a sequence of independent Bernoulli trials with probability p of success on each trial; number of items inspected before encountering the first defective item; number of items in a batch of random size; number of items demanded from an inventory.) A valid probability have to be a value between 0 and 1."
      )]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Probability
    {
      get;
      set;
    }
  }
}