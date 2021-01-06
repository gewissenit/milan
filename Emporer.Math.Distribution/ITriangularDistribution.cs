#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  public interface ITriangularDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the mean value of this Triangular distribution. This value must be between the lower border and the
    ///   upper border to be valid.
    /// </summary>
    /// <value>The mean value of this Triangular distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Mean")]
    [Category("Parameters")]
    [Description("The mean value of the triangular distribution. This value must be between the lower border and the upper border to be valid.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Mean { get; set; }

    /// <summary>
    ///   Gets or sets the upper border of the range of this triangular distribution.
    /// </summary>
    /// <value>The upper border of the range of this triangular distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Upper border")]
    [Category("Parameters")]
    [Description("The upper border of the range of the triangular distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double UpperBorder { get; set; }

    /// <summary>
    ///   Gets or sets the lower border of the range of this triangular distribution.
    /// </summary>
    /// <value>The lower border of the range of this triangular distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Lower border")]
    [Category("Parameters")]
    [Description("The lower border of the range of the triangular distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double LowerBorder { get; set; }
  }
}