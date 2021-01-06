#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

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
  public class BetaDistribution : Distribution, IBetaDistribution, IRealDistribution
  {
    #region Gamma Distributions

    private GammaDistribution _FirstGammaDistribution;

    private GammaDistribution _SecondGammaDistribution;

    internal GammaDistribution FirstGammaDistribution
    {
      get
      {
        if (_FirstGammaDistribution == null)
        {
          _FirstGammaDistribution = new GammaDistribution(FirstShape, 1)
                                     {
                                       Seed = Seed,
                                       IsAntithetic = IsAntithetic
                                     };
        }
        return _FirstGammaDistribution;
      }
      set { _FirstGammaDistribution = value; }
    }

    internal GammaDistribution SecondGammaDistribution
    {
      get
      {
        if (_SecondGammaDistribution == null)
        {
          _SecondGammaDistribution = new GammaDistribution(SecondShape, 1)
                                      {
                                        Seed = (Seed + 100),
                                        IsAntithetic = IsAntithetic
                                      };
        }
        return _SecondGammaDistribution;
      }

      set { _SecondGammaDistribution = value; }
    }

    #endregion
    
    #region SetShapes

    /// <summary>
    ///   Note that shape1 and shape2 parameter has to be > 0. Also that max must be greater than min.
    ///   The shapes of the underlying gamma distributions will be create by the three parameters a,m and b.
    ///   first shape= 1 + 4 * ((m - a) / (b - a));
    ///   second shape = = 6 - first shape;
    /// </summary>
    /// <param name="min">min</param>
    /// <param name="median">median</param>
    /// <param name="max">max</param>
    public void SetShapes(double min, double median, double max)
    {
      if (!((min <= median) && (median <= max)) ||
          (min == max))
      {
        var msg =
          string.Format(
                        "The parameters of the Beta distribution must adhere to a <= m <= b and a must not equal b. The given parameters (a: {0}, m: {1}, b: {2}) are invalid.",
                        min,
                        median,
                        max);
        throw new InvalidOperationException(msg);
      }
      FirstShape = 1 + 4 * ((median - min) / (max - min));
      SecondShape = 6 - FirstShape;
      Minimum = min;
      Maximum = max;
    }

    #endregion

    #region Shapes

    private double _FirstShape;

    private double _SecondShape;

    /// <summary>
    ///   Get/Set the first shape for the Beta distribution. If X and Y are independently distributed Gamma(a, c) and Gamma(b,
    ///   c) respectively, then X / (X + Y) has a beta distribution with parameters a and b. a is the FirstShape and b is the
    ///   SecondShape.
    /// </summary>
    /// <value>FirstShape</value>
    public double FirstShape
    {
      get { return _FirstShape; }
      set
      {
        _FirstShape = value;
        IsValid = CheckValues();
      }
    }

    /// <summary>
    ///   Get/Set the second shape for the beta distribution. If X and Y are independently distributed Gamma(a, c) and Gamma(b,
    ///   c) respectively, then X / (X + Y) has a beta distribution with parameters a and b. a is the FirstShape and b is the
    ///   SecondShape.
    /// </summary>
    /// <value>SecondShape</value>
    public double SecondShape
    {
      get { return _SecondShape; }
      set
      {
        _SecondShape = value;
        IsValid = CheckValues();
      }
    }

    #endregion

    #region Minimum

    private double _Minimum;

    /// <summary>
    ///   Gets the minimum parameter for the beta distribution.
    /// </summary>
    /// <value>The minimum.</value>
    public double Minimum
    {
      get { return _Minimum; }
      set
      {
        _Minimum = value;
        IsValid = CheckValues();
      }
    }

    #endregion

    #region Maximum

    private double _Maximum = 1.0;

    /// <summary>
    ///   Gets the maximum parameter for the beta distribution.
    /// </summary>
    /// <value>The maximum.</value>
    public double Maximum
    {
      get { return _Maximum; }
      set
      {
        _Maximum = value;
        IsValid = CheckValues();
      }
    }

    #endregion

    #region IsAntithetic

    /// <summary>
    ///   Gets or sets a value indicating whether this <see cref="BetaDistribution" /> is antithetic.
    /// </summary>
    /// <value><c>true</c> if antithetic; otherwise, <c>false</c>.</value>
    public override bool IsAntithetic
    {
      get { return base.IsAntithetic; }
      set
      {
        // SetAnithetic to underlying Distributions
        base.IsAntithetic = value;
        if (_SecondGammaDistribution != null)
        {
          _SecondGammaDistribution.IsAntithetic = value;
        }
        if (_FirstGammaDistribution != null)
        {
          _FirstGammaDistribution.IsAntithetic = value;
        }
      }
    }

    #endregion
    
    #region GetSample

    /// <summary>
    ///   Returns the next floating point Sample from this beta distribution.
    /// </summary>
    /// <returns>The next floating point Sample from this beta distribution</returns>
    public double GetSample()
    {
      if (IsValid)
      {
        var firstGammaSample = (FirstGammaDistribution as IRealDistribution).GetSample();
        var secondGammaSample = (SecondGammaDistribution as IRealDistribution).GetSample();
        var result = firstGammaSample / (firstGammaSample + secondGammaSample);
        return _Minimum + (_Maximum - _Minimum) * result;
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (Maximum < Minimum)
      {
        messages.Append("Please attend that Minimum value is not greater than the Maximum value.");
      }
      return messages;
    }

    private bool CheckValues()
    {
      return Maximum >= Minimum && SecondShape >= FirstShape;
    }
  }
}