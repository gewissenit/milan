#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Poisson distributed stream of pseudo random integer numbers. The distribution is specified by one parameter
  ///   describing the mean value.
  /// </summary>
  public class PoissonDistribution : Distribution, IPoissonDistribution, IIntDistribution, IRealDistribution
  {
    #region ExpMean

    private double ExpMean { get; set; }

    #endregion

    #region Mean

    private double _Mean;

    /// <summary>
    ///   Gets or sets the mean value of the poisson distribution.
    /// </summary>
    /// <value>The mean.</value>
    public double Mean
    {
      get { return _Mean; }
      set
      {
        _Mean = value;
        ExpMean = System.Math.Exp(-Mean);
        IsValid = !(Double.IsNaN(ExpMean) || Double.IsInfinity(ExpMean) || ExpMean <= 0 || value <= 0);
      }
    }

    #endregion

    #region Sample

    /// <summary>
    ///   Abstract method should return the specific Sample as a integer value when implemented in subclasses.
    ///   Returns the next poisson distributed Sample from this distribution.
    ///   The algorithm used is taken from DESMO-C-Framework from Thomas Schniewind [Schn98] Volume 2,
    ///   page103, file intdist.cc. It has been adopted and extended to handle
    ///   antithetic random numbers if antithetic mode is switched on.
    /// </summary>
    /// <returns>
    ///   The next Poison distributed Sample to be drawn from this distribution
    /// </returns>
    int IIntDistribution.GetSample()
    {
      var q = 1.0;
      var result = -1;

      if (IsValid)
      {
        if (IsAntithetic)
        {
          do
          {
            q = q * (1 - RandomGenerator.NextDouble());
            result = result + 1;
          } while (q >= ExpMean);
        }
        else
        {
          do
          {
            q = q * RandomGenerator.NextDouble();
            result = result + 1;
          } while (q >= ExpMean);
        }
        return result;
      }
      throw new InvalidOperationException(Validate());
    }

    double IRealDistribution.GetSample()
    {
      return Convert.ToDouble((this as IIntDistribution).GetSample());
    }

    #endregion

    #region ValidateCore

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (Double.IsNaN(ExpMean))
      {
        messages.Append("Your mean produce a NaN value.");
      }
      if (Double.IsInfinity(ExpMean))
      {
        messages.Append("Your mean produce a NaN value.");
      }
      if (ExpMean <= 0)
      {
        messages.Append("Your mean produce an invalid value.");
      }
      return messages;
    }

    #endregion
  }
}