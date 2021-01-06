#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Negative-exponential distributed stream of pseudo random numbers of type double.
  /// </summary>
  public class ExponentialDistribution : Distribution, IExponentialDistribution, IRealDistribution
  {
    #region Minimum

    private double _Minimum;

    /// <summary>
    ///   Gets or sets the minimum for this distribution
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
    ///   Gets or sets the mean of the negative-exponential distribution. Only a non-negative value is a valid mean.
    /// </summary>
    /// <value>The mean.</value>
    public double Mean
    {
      get { return _Mean; }
      set
      {
        _Mean = value;
        IsValid = (value >= 0);
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
    ///   Returns the next negative exponential pseudo random number.
    ///   The algorithm used is taken from DESMO-C from Thomas Schniewind [Schni98] Volume 2,
    ///   page 221, file realdist.cc. It has been adapted and extended to handle
    ///   antithetic random numbers if antithetic mode is switched on.
    /// </summary>
    /// <returns>The next negative exponential pseudo random number</returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        if ((IsAntithetic))
        {
          return Minimum + (-System.Math.Log(1 - RandomGenerator.NextDouble()) * Mean);
        }
        return Minimum + (-System.Math.Log(RandomGenerator.NextDouble()) * Mean);
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (Mean < 0)
      {
        messages.Append("Please check the mean of the distribution. Only a non-negative value for the mean is valid.");
      }
      return messages;
    }
  }
}