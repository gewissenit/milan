#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Uniform distributed stream of pseudo random numbers. Values produced by this
  ///   distribution are uniformly distributed in the range specified by parameters.
  /// </summary>
  public class UniformDistribution : Distribution, IUniformDistribution, IRealDistribution, IUniformIntDistribution
  {
    #region UpperBorder

    private double _DoubleUpperBorder;

    private int _IntUpperBorder;

    /// <summary>
    ///   Gets or sets the upper border of the range of this uniform distribution.
    /// </summary>
    /// <value>The upper border of the range of this uniform distribution.</value>
    double IUniformDistribution.UpperBorder
    {
      get { return _DoubleUpperBorder; }
      set
      {
        _DoubleUpperBorder = value;
        IsValid = _DoubleLowerBorder < _DoubleUpperBorder;
      }
    }

    /// <summary>
    ///   Gets or sets the upper border of the range of this uniform distribution.
    /// </summary>
    /// <value>The upper border of the range of this uniform distribution.</value>
    int IUniformIntDistribution.UpperBorder
    {
      get { return _IntUpperBorder; }
      set
      {
        _IntUpperBorder = value;
        _DoubleUpperBorder = value;
        IsValid = _IntLowerBorder < _IntUpperBorder;
      }
    }

    #endregion

    #region LowerBorder

    private double _DoubleLowerBorder;

    private int _IntLowerBorder;

    /// <summary>
    ///   Gets or sets the lower border of the range of this uniform distribution.
    /// </summary>
    /// <value>The lower border of the range of this uniform distribution.</value>
    double IUniformDistribution.LowerBorder
    {
      get { return _DoubleLowerBorder; }
      set
      {
        _DoubleLowerBorder = value;
        IsValid = _DoubleLowerBorder < _DoubleUpperBorder;
      }
    }

    /// <summary>
    ///   Gets or sets the lower border of the range of this uniform distribution.
    /// </summary>
    /// <value>The lower border of the range of this uniform distribution.</value>
    int IUniformIntDistribution.LowerBorder
    {
      get { return _IntLowerBorder; }
      set
      {
        _IntLowerBorder = value;
        _DoubleLowerBorder = value;
        IsValid = _IntLowerBorder < _IntUpperBorder;
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
    ///   Returns the next floating point Sample from this uniform distribution.
    ///   The value returned is basically the uniformly distributed pseudo random
    ///   number produced by the underlying random generator stretched to match the
    ///   range specified by parameters.
    /// </summary>
    /// <returns>
    ///   The floating point Sample to be drawn from this distribution.
    /// </returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        double randomNumber = RandomGenerator.NextDouble();
        if (IsAntithetic)
        {
          //return _DoubleLowerBorder + ((_DoubleUpperBorder - _DoubleLowerBorder) * (1 - RandomGenerator.NextDouble()));
          return (1 - randomNumber) * _DoubleUpperBorder + randomNumber * _DoubleLowerBorder;
        }

        //return _DoubleLowerBorder + ((_DoubleUpperBorder - _DoubleLowerBorder) * randomNumber);
        return randomNumber * _DoubleUpperBorder + (1 - randomNumber) * _DoubleLowerBorder;
      }
      throw new InvalidOperationException(Validate());
    }

    /// <summary>
    ///   Returns the next floating point Sample from this uniform distribution.
    ///   The value returned is basically the uniformly distributed pseudo random
    ///   number produced by the underlying random generator stretched to match the
    ///   range specified by parameters.
    /// </summary>
    /// <returns>
    ///   The int Sample to be drawn from this distribution.
    /// </returns>
    int IIntDistribution.GetSample()
    {
      if (IsValid)
      {
        double randomNumber = RandomGenerator.NextDouble();
        if (IsAntithetic)
        {
          //return _IntLowerBorder + (Int32)((_IntUpperBorder - _IntLowerBorder + 1) * (1 - RandomGenerator.NextDouble()));
          return Convert.ToInt32((1 - randomNumber) * _IntUpperBorder + randomNumber * _IntLowerBorder);
        }
        //return _IntLowerBorder + (Int32)((_IntUpperBorder - _IntLowerBorder + 1) * RandomGenerator.NextDouble());
        return Convert.ToInt32(randomNumber * _IntUpperBorder + (1 - randomNumber) * _IntLowerBorder);
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (! (_DoubleLowerBorder < _DoubleUpperBorder))
      {
        messages.Append(
                        "Please check the paramter of the distribution. The condition that the LowerBorder is smaller than the UpperBorder must be meeted.");
      }
      return messages;
    }
  }
}