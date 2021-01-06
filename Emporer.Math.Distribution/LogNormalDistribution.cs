#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Log Normal distributed stream of pseudo random numbers of type double.
  /// </summary>
  public class LogNormalDistribution : Distribution, ILogNormalDistribution, IRealDistribution
  {
    #region NormalDistribution

    private NormalDistribution _NormalDistribution;

    /// <summary>
    ///   Normal distribution used to calculate the Log Normal Distribution
    /// </summary>
    internal NormalDistribution NormalDistribution
    {
      get
      {
        if (_NormalDistribution == null)
        {
          _NormalDistribution = new NormalDistribution();
          SetNormalDistributionParameters();
        }
        return _NormalDistribution;
      }
      set { _NormalDistribution = value; }
    }

    #endregion

    #region Mean

    private double _Mean;

    /// <summary>
    ///   Gets or sets the mean value of this Log Normal distribution.
    /// </summary>
    /// <value>The mean value of this Log Normal distribution.</value>
    public double Mean
    {
      get { return _Mean; }
      set
      {
        _Mean = value;
        SetNormalDistributionParameters();
      }
    }

    #endregion

    #region StdDev

    private double _StdDev;

    /// <summary>
    ///   Gets or sets the standard deviation of this Log Normal distribution.
    /// </summary>
    /// <value>The standard deviation of this Log Normal distribution..</value>
    public double StandardDeviation
    {
      get { return _StdDev; }
      set
      {
        _StdDev = value;
        SetNormalDistributionParameters();
      }
    }

    #endregion

    private void SetNormalDistributionParameters()
    {
      if (_NormalDistribution != null)
      {
        if (ParamsNormal)
        {
          _NormalDistribution.Mean = Mean;
          _NormalDistribution.StandardDeviation = StandardDeviation;
        }
        else
        {
          _NormalDistribution.Mean = CalculateNormalDistributionMean(Mean, StandardDeviation);
          _NormalDistribution.StandardDeviation = CalculateNormalDistributionStdDev(Mean, StandardDeviation);
        }
      }
    }

    #region Minimum

    private double _Minimum;

    /// <summary>
    ///   Gets or sets the minimum for this distribuiton.
    /// </summary>
    /// <value>The minimum.</value>
    public double Minimum
    {
      get { return _Minimum; }
      set { _Minimum = value; }
    }

    #endregion

    #region ParamsNormal

    private bool _ParamsNormal;

    /// <summary>
    ///   Gets or sets a value indicating whether [params normal].
    /// </summary>
    /// <value><c>true</c> if [params normal]; otherwise, <c>false</c>.</value>
    public bool ParamsNormal
    {
      get { return _ParamsNormal; }
      set
      {
        if (_ParamsNormal != value)
        {
          _ParamsNormal = value;
          SetNormalDistributionParameters();
        }
      }
    }

    #endregion
    
    #region IsValid

    internal override bool IsValid
    {
      set { base.IsValid = value; }
    }

    #endregion

    #region Seed

    /// <summary>
    ///   Gets or sets the seed of the underlying pseudorandom generator.
    ///   The seed value is passed on to the underlying RandomGenerator but since those generators are not supposed to keep
    ///   track of their initial
    ///   seed value it is stored here to make sure they are not lost.
    ///   The seed controls the starting value of the random generators and all
    ///   following generated pseudo random numbers.
    ///   Resetting the seed between two simulation runs will let you use identical
    ///   streams of random numbers. That will enable you to compare different
    ///   strategies within your model based on the same random number stream produced
    ///   by the random generator.
    /// </summary>
    /// <value>The seed.</value>
    public override long Seed
    {
      get { return NormalDistribution.RandomGenerator.Seed; }
      set { NormalDistribution.RandomGenerator.Seed = value; }
    }

    #endregion

    #region CalculateNormalDistributionMean

    /// <summary>
    ///   Calculates the Mean-Value used for the underlying Normal Distribution.
    /// </summary>
    /// <param name="Mean">The mean.</param>
    /// <param name="StdDev">The STD dev.</param>
    /// <returns></returns>
    private double CalculateNormalDistributionMean(double Mean, double StdDev)
    {
      if (Mean != 0.0)
      {
        return System.Math.Log(System.Math.Pow(Mean, 2) / System.Math.Sqrt(System.Math.Pow(StdDev, 2) + System.Math.Pow(Mean, 2)));
      }
      else
      {
        return 0.0;
      }
    }

    #endregion

    #region CalculateNormalDistributionStdDev

    /// <summary>
    ///   Calculates the Standard Derivation-Value used for the underlying Normal Distribution
    /// </summary>
    /// <param name="Mean">The mean.</param>
    /// <param name="StdDev">The STD dev.</param>
    /// <returns></returns>
    private double CalculateNormalDistributionStdDev(double Mean, double StdDev)
    {
      return System.Math.Log((System.Math.Pow(StdDev, 2) / System.Math.Pow(Mean, 2) + 1));
    }

    #endregion

    #region Sample

    /// <summary>
    ///   Returns the next Log Normal distributed Sample from this distribution.
    ///   The value depends upon the seed, the number of values taken from the
    ///   stream by using this method before and the mean and standard deviation
    ///   values specified for this distribution.
    /// </summary>
    /// <returns>The next Log Normal distributed Sample from this distribution.</returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        return System.Math.Exp((NormalDistribution as IRealDistribution).GetSample()) + _Minimum;
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion
  }
}