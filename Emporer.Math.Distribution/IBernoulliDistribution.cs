#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Boolean Bernoulli distribution returning true values with the
  ///   given probability. Samples of this distribution can either be true or false
  ///   with a given probability
  ///   for value "true". The probabilitiy for "true" can only be set via the constructor.
  ///   It has to be a value between 0 and 1.
  ///   1.0 as values mean that always return "true".
  ///   0.0 as value mean that always return "false".
  /// </summary>
  public interface IBernoulliDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the probability for true values as double
    ///   for maximum precision.
    /// </summary>
    /// <value>The probability of a true value being returned.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Probability")]
    [Category("Parameters")]
    [Description("The probability of a true value being returned.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Probability { get; set; }
  }
}