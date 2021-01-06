#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Erlang distributed stream of pseudo random numbers of type double.
  ///   Erlang distributed streams are specified by a mean value and their order.
  /// </summary>
  public class ErlangDistribution : Distribution, IErlangDistribution, IRealDistribution
  {
    #region Minimum

    private double _Minimum;

    /// <summary>
    ///   Gets or sets the minimum for this distribution. The adjustment on the variableValue-axis
    /// </summary>
    /// <value>The minimum.</value>
    public double Minimum
    {
      get { return _Minimum; }
      set { _Minimum = value; }
    }

    #endregion

    #region Mean

    private double _Mean;

    /// <summary>
    ///   Gets or sets the mean value of this Erlang distribution.
    /// </summary>
    /// <value>The mean value of this Erlang distribution.</value>
    public double Mean
    {
      get { return _Mean; }
      set
      {
        _Mean = value;
        IsValid = (Mean >= 0 && Order > 0);
      }
    }

    #endregion

    #region Order

    private int _Order;

    /// <summary>
    ///   Gets or sets the order of the Erlang distribution.
    /// </summary>
    /// <value>The order.</value>
    public int Order
    {
      get { return _Order; }
      set
      {
        _Order = value;
        IsValid = (Mean >= 0 && Order > 0);
      }
    }

    #endregion

    #region IsValid

    internal override bool IsValid
    {
      set { base.IsValid = value; }
    }

    #endregion

    #region Sample

    /// <summary>
    ///   Returns the next Erlang distributed Sample from this distribution. The algorithm used is taken from DESMO-C
    ///   from Thomas Schniewind [Schni98] Volume 2, page 222, file realdist.cc. It has been adapted and extended to
    ///   handle antithetic random numbers if antithetic mode is switched on.
    /// </summary>
    /// <returns>The next Erlang distributed Sample.</returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        var q = 1.0;
        if (IsAntithetic)
        {
          for (var i = 1; i <= Order; i++)
          {
            q = q * (1 - RandomGenerator.NextDouble());
          }
        }
        else
        {
          for (var i = 1; i <= Order; i++)
          {
            q = q * RandomGenerator.NextDouble();
          }
        }
        return ((-System.Math.Log(q) * Mean) / Order) + Minimum;
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (Mean < 0)
      {
        messages.Append("The mean parameter of the ErlangDistribution is not a valid value, because it is lower than 0.");
      }
      if (Order < 1)
      {
        messages.Append("The order parameter of the ErlangDistributionr is not a valid value, because it is not greater than 0.");
      }
      return messages;
    }
  }
}