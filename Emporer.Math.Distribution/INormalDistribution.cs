#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Normal (a.k.a. "Gaussian") distributed stream of pseudo random numbers of type double.
  /// </summary>
  public interface INormalDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the mean value of this Normal (a.k.a. "Gaussian") distribution.
    /// </summary>
    /// <value>The mean value of this Normal (a.k.a. "Gaussian") distribution.</value>
    [ReadOnly(false)]
    [DisplayName("Mean")]
    [Category("Parameters")]
    [Description("The mean value of this Normal (a.k.a. 'Gaussian') distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    double Mean { get; set; }

    /// <summary>
    ///   Gets or sets the standard deviation of this normal (a.k.a. "Gaussian") distribution.
    /// </summary>
    /// <value>The standard deviation of this normal (a.k.a. "Gaussian") distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("StandardDeviation")]
    [Category("Parameters")]
    [Description("The standard deviation of this normal (a.k.a. 'Gaussian') distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double StandardDeviation { get; set; }
  }
}