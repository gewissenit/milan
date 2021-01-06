#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Weibull distributed stream of pseudo random numbers of type double.
  /// </summary>
  public class WeibullDistribution : Distribution, IWeibullDistribution, IRealDistribution
  {
    #region Scale

    private double _Scale;

    /// <summary>
    ///   Gets or sets the scale parameter for the weibull distribution.
    ///   Note that shape and scale parameter has to be greater 0.
    /// </summary>
    /// <value>The scale parameter for the weibull distribution.</value>
    public double Scale
    {
      get { return _Scale; }
      set
      {
        _Scale = value;
        IsValid = (_Scale >= 0) && (_Shape >= 0);
      }
    }

    #endregion

    #region Shape

    private double _Shape;

    /// <summary>
    ///   Gets or sets the shape parameter for the weibull distribution.
    ///   Note that shape and scale parameter has to be greater 0.
    /// </summary>
    /// <value>The shape parameter (alpha) for the weibull distribution.</value>
    public double Shape
    {
      get { return _Shape; }
      set
      {
        _Shape = value;
        IsValid = (_Scale >= 0) && (_Shape >= 0);
      }
    }

    #endregion
    
    #region IsValid

    internal override bool IsValid
    {
      set { base.IsValid = value; }
    }

    #endregion

    #region Sample (override)

    /// <summary>
    ///   Returns the next floating point Sample from this weibull distribution.
    /// </summary>
    /// <returns>
    ///   The floating point Sample to be drawn from this distribution.
    /// </returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        if ((IsAntithetic))
        {
          return Scale * System.Math.Pow(-System.Math.Log(1 - RandomGenerator.NextDouble()), 1 / Shape);
        }
        return Scale * System.Math.Pow(-System.Math.Log(RandomGenerator.NextDouble()), 1 / Shape);
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (Shape < 0 ||
          Scale < 0)
      {
        messages.Append("Please check the paramter of the distribution. It is not allowed that Shape or Scale be lower than zero.");
      }
      return messages;
    }
  }
}