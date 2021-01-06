#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Erlang distributed stream of pseudo random numbers of type double.
  ///   Erlang distributed streams are specified by a mean value and their order.
  /// </summary>
  public interface IErlangDistribution : IDistribution
  {
    /// <summary>
    ///   Gets or sets the minimum for this distribution. The adjustment on the variableValue-axis
    /// </summary>
    /// <value>The minimum.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Minimum")]
    [Category("Parameters")]
    [Description("The adjustment on the variableValue-axis.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Minimum { get; set; }

    /// <summary>
    ///   Gets or sets the mean value of this Erlang distribution. This value mustn't be negative to be valid.
    /// </summary>
    /// <value>The mean value of this Erlang distribution.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Mean")]
    [Category("Parameters")]
    [Description("The mean value of this Erlang distribution. This value mustn't be negative to be valid.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double Mean { get; set; }

    /// <summary>
    ///   Gets or sets the order of the Erlang distribution. A valid value for the order have to be greater than 0.
    /// </summary>
    /// <value>The order.</value>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Order")]
    [Category("Parameters")]
    [Description("The order value of this Erlang distribution. A valid value for the order have to be greater than 0.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    int Order { get; set; }
  }
}