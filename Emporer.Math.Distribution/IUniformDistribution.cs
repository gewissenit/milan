#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Uniform distributed stream of pseudo random numbers. Values produced by this
  ///   distribution are uniformly distributed in the range specified by parameters.
  /// </summary>
  public interface IUniformDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the upper border of the range of this uniform distribution.
    /// </summary>
    /// <value>The upper border of the range of this uniform distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Upper border")]
    [Category("Parameters")]
    [Description("The upper border of the range of this uniform distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double UpperBorder { get; set; }

    /// <summary>
    ///   Gets or sets the lower border of the range of this uniform distribution.
    /// </summary>
    /// <value>The lower border of the range of this uniform distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Lower border")]
    [Category("Parameters")]
    [Description("The lower border of the range of this uniform distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double LowerBorder { get; set; }
  }
}