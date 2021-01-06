#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Log Normal distributed stream of pseudo random numbers of type double.
  /// </summary>
  public interface ILogNormalDistribution : INormalDistribution
  {
    /// <summary>
    ///   Gets or sets the minimum for this distribuiton.
    /// </summary>
    /// <value>The minimum.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Minimum")]
    [Category("Parameters")]
    [Description("The minimum of this lognormal distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Minimum { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether [params normal].
    /// </summary>
    /// <value><c>true</c> if [params normal]; otherwise, <c>false</c>.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("ParamsNormal")]
    [Category("Parameters")]
    [Description("ParamsNormal of this lognormal distribution. value is true if [params normal], otherwise false.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    bool ParamsNormal { get; set; }
  }
}