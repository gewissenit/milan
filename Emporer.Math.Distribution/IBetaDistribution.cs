#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Beta distributed stream of pseudo random numbers of type double.
  ///   Use the Gamma distribution for applications as a rough model in the absence of
  ///   data.
  ///   The Beta(a,b)-distribution is define as a distribution with G / (G + H),
  ///   where G is a gamma-distribution with value a and H ist a gamma-distribution with value b.
  ///   So this beta-distribution-implementation use two gamma-distributions to get a sample.
  /// </summary>
  public interface IBetaDistribution : IDistribution
  {
    /// <summary>
    ///   Gets the minimum parameter for the beta distribution.
    /// </summary>
    /// <value>The minimum.Default by 0.0</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Minimum")]
    [Category("Parameters")]
    [Description("The minimum for the beta distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Minimum { get; set; }

    /// <summary>
    ///   Gets the maximum parameter for the beta distribution.
    /// </summary>
    /// <value>The maximum. Default by 1.0</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Maximum")]
    [Category("Parameters")]
    [Description("The maximum for the beta distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Maximum { get; set; }

    /// <summary>
    ///   Get/Set the first shape for the Beta distribution. If X and Y are independently distributed Gamma(a, c) and Gamma(b,
    ///   c) respectively, then X / (X + Y) has a beta distribution with parameters a and b. a is the FirstShape and b is the
    ///   SecondShape.
    /// </summary>
    /// <value>FirstShape</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("First Shape")]
    [Category("Parameters")]
    [Description("The shape of the first underlying gamma distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double FirstShape { get; set; }

    /// <summary>
    ///   Get/Set the second shape for the beta distribution. If X and Y are independently distributed Gamma(a, c) and Gamma(b,
    ///   c) respectively, then X / (X + Y) has a beta distribution with parameters a and b. a is the FirstShape and b is the
    ///   SecondShape.
    /// </summary>
    /// <value>SecondShape</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Second Shape")]
    [Category("Parameters")]
    [Description("The shape of the second underlying gamma distribution.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double SecondShape { get; set; }

    /// <summary>
    ///   Note that shape1 and shape2 parameter has to be > 0.
    ///   The shapes of the underlying gamma distributions will be create by the three parameters a,m and b.
    ///   This method allows you to set both shapes simultaneously with the follow algorithm:
    ///   first shape= 1 + 4 * ((m - a) / (b - a));
    ///   second shape = = 6 - first shape;
    /// </summary>
    /// <param name="min">min</param>
    /// <param name="median">median</param>
    /// <param name="max">max</param>
    void SetShapes(double min, double median, double max);
  }
}