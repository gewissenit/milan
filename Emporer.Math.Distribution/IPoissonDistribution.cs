#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Poisson distributed stream of pseudo random integer numbers. The distribution is specified by one parameter
  ///   describing the mean value.
  /// </summary>
  public interface IPoissonDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the mean value of the poisson distribution.
    /// </summary>
    /// <value>The mean.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Mean")]
    [Category("Parameters")]
    [Description("The mean value of the poisson distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Mean { get; set; }
  }
}