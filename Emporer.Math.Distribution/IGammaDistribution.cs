#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  public interface IGammaDistribution : IDistribution
  {
    /// <summary>
    ///   Getter and Setter fot the minimum parameter, where the distribution starts
    ///   on the variableValue-axis. This value mustn't be negative to be valid.
    /// </summary>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Minimum")]
    [Category("Parameters")]
    [Description("The minimum of the gamma distribution. This value mustn't be negative to be valid.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Minimum { get; set; }

    /// <summary>
    ///   Gets or sets the scale or beta parameter for the gamma distribution. This value mustn't be negative to be valid.
    /// </summary>
    /// <value>The scale parameter (beta) for the gamma distribution</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Scale")]
    [Category("Parameters")]
    [Description("The scale parameter (beta) for the gamma distribution. This value mustn't be negative to be valid.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Scale { get; set; }

    /// <summary>
    ///   Gets or sets the shape or alpha parameter for the Gamma distribution. This value mustn't be negative to be valid.
    /// </summary>
    /// <value>The shape parameter (alpha) for the Gamma distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Shape")]
    [Category("Parameters")]
    [Description("The shape parameter (alpha) for the Gamma distribution. This value mustn't be negative to be valid.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Shape { get; set; }
  }
}