#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Normal (a.k.a. "Gaussian") distributed stream of pseudo random numbers of type double.
  /// </summary>
  public class NormalDistribution : Distribution, INormalDistribution, IRealDistribution
  {
    #region HaveNextGaussian

    /// <summary>
    ///   Flag indicates wether there is already a calculated Gaussian value present in the buffer variable nextGaussian.
    ///   When calculating a pseudo random number using the algorithm implemented here, two Gaussian values are computed at a
    ///   time,
    ///   so the other value is stored to be used next time the client asks for a new value, thus saving on computation time.
    ///   If true, there is a next Gaussian value already calculated, if false a new pair of Gaussian values has to be
    ///   generated.
    /// </summary>
    internal bool HaveNextGaussian
    {
      get;
      set;
    }

    #endregion

    #region NextGaussian

    /// <summary>
    ///   Buffer for storing the next gaussian value already calculated. When computing a Gaussian value
    ///   two Samples of a pseudo random number stream are taken and calculated to produce two gaussian values,
    ///   even if only one is used.
    ///   So the other value is stored to be delivered next time a Gaussian value is requested by a client.
    /// </summary>
    internal double NextGaussian
    {
      get;
      set;
    }

    #endregion

    #region Mean

    private double _Mean;

    /// <summary>
    ///   Gets or sets the mean value of this Normal (a.k.a. "Gaussian") distribution.
    /// </summary>
    /// <value>The mean value of this Normal (a.k.a. "Gaussian") distribution.</value>
    public double Mean
    {
      get { return _Mean; }
      set
      {
        _Mean = value;
        IsValid = true;
      }
    }

    #endregion

    #region StdDev

    private double _StandardDeviation;

    /// <summary>
    ///   Gets or sets the standard deviation of this normal (a.k.a. "Gaussian") distribution.
    /// </summary>
    /// <value>The standard deviation of this normal (a.k.a. "Gaussian") distribution.</value>
    public double StandardDeviation
    {
      get { return _StandardDeviation; }
      set
      {
        _StandardDeviation = value;
        IsValid = true;
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
    ///   Returns the next normal (a.k.a. "Gaussian") distributed Sample from this distribution.
    ///   The value depends upon the seed, the number of values taken from the stream by using this
    ///   method before and the mean and standard deviation values specified for this distribution.
    /// </summary>
    /// <returns>The next normal (a.k.a. "Gaussian") distributed Sample from this distribution.</returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        if (HaveNextGaussian)
        {
          HaveNextGaussian = false;
          return NextGaussian;
        }
        else
        {
          double firstRandomValue;
          double secondRandomValue;
          double intermediateResult;
          if ((IsAntithetic))
          {
            do
            {
              firstRandomValue = 2 * (1 - RandomGenerator.NextDouble()) - 1;
              secondRandomValue = 2 * (1 - RandomGenerator.NextDouble()) - 1;
              intermediateResult = firstRandomValue * firstRandomValue + secondRandomValue * secondRandomValue;
            } while (intermediateResult >= 1);
          }
          else
          {
            do
            {
              firstRandomValue = 2 * (RandomGenerator.NextDouble()) - 1;
              secondRandomValue = 2 * (RandomGenerator.NextDouble()) - 1;
              intermediateResult = firstRandomValue * firstRandomValue + secondRandomValue * secondRandomValue;
            } while (intermediateResult >= 1);
          }
          var multiplier = System.Math.Sqrt(-2 * System.Math.Log(intermediateResult) / intermediateResult);
          NextGaussian = secondRandomValue * multiplier * StandardDeviation + Mean;
          HaveNextGaussian = true;
          return (firstRandomValue * multiplier) * _StandardDeviation + Mean;
        }
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion
  }
}