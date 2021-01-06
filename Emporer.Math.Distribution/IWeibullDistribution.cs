#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Weibull distributed stream of pseudo random numbers of type double.
  /// </summary>
  public interface IWeibullDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the scale parameter for the weibull distribution. A valid scale have a value >= 0.
    /// </summary>
    /// <value>The scale parameter for the weibull distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Scale")]
    [Category("Parameters")]
    [Description("The scale parameter for the weibull distribution. A valid scale have a value >= 0.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Scale { get; set; }


    /// <summary>
    ///   Gets or sets the shape parameter for the weibull distribution. A valid shape have a value >= 0.
    /// </summary>
    /// <value>The shape parameter (alpha) for the weibull distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Shape")]
    [Category("Parameters")]
    [Description("The shape parameter (alpha) for the weibull distribution. A valid shape have a value >= 0.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Shape { get; set; }
  }
}