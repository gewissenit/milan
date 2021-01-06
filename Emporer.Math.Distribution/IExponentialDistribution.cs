#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Negative-exponential distributed stream of pseudo random numbers of type double.
  /// </summary>
  public interface IExponentialDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the minimum for this distribution
    /// </summary>
    /// <value>The minimum.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Minimum")]
    [Category("Parameters")]
    [Description("The minimum of the exponential distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Minimum { get; set; }

    /// <summary>
    ///   Gets or sets the mean of the negative-exponential distribution. Only a non-negative value is a valid mean.
    /// </summary>
    /// <value>The mean.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Mean")]
    [Category("Parameters")]
    [Description("The mean of the negative-exponential distribution. Only a non-negative value is a valid mean.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Mean { get; set; }
  }
}