#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Triangular distributed stream of pseudo random numbers.
  ///   Values produced by this distribution are triangular distributed in the range specified by parameters.
  /// </summary>
  public class TriangularDistribution : Distribution, ITriangularDistribution, IRealDistribution, IIntDistribution
  {
    #region Constructor(s)

    /// <summary>
    ///   Initializes a new instance of the <see cref="TriangularDistribution" /> class, a pseudo random numbers
    ///   following a triangular distribution.
    ///   The specific lower and upper borders of the range of this distribution together
    ///   with then mean parameter c have to be given at creation time.
    ///   Note that the lower border in fact has to be lower than the upper border.
    ///   Furthermore the three parameters have to meet the condition that lower border smaller than meanValue smaller than
    ///   upper border.
    /// </summary>
    public TriangularDistribution()
    {
      _IsValid = CheckTriangularCondition();
    }

    #endregion
    
    #region Mean

    private double _Mean;

    double ITriangularDistribution.Mean
    {
      get { return _Mean; }
      set
      {
        _Mean = value;
        IsValid = CheckTriangularCondition();
      }
    }

    #endregion

    #region UpperBorder

    private double _UpperBorder;

    double ITriangularDistribution.UpperBorder
    {
      get { return _UpperBorder; }
      set
      {
        _UpperBorder = value;
        IsValid = CheckTriangularCondition();
      }
    }

    #endregion

    #region LowerBorder

    private double _LowerBorder;

    double ITriangularDistribution.LowerBorder
    {
      get { return _LowerBorder; }
      set
      {
        _LowerBorder = value;
        IsValid = CheckTriangularCondition();
      }
    }

    #endregion

    int IIntDistribution.GetSample()
    {
      return Convert.ToInt32((this as IRealDistribution).GetSample());
    }

    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        double Spread = _UpperBorder - _LowerBorder;
        double relation = _Mean - _LowerBorder;
        var uMean = relation / Spread;
        double u = IsAntithetic
                     ? (1 - RandomGenerator.NextDouble())
                     : (RandomGenerator.NextDouble());
        u = (u <= uMean)
              ? (System.Math.Pow(u * uMean, 0.5))
              : (1.0 - System.Math.Pow((1.0 - uMean) * (1.0 - u), 0.5));
        return _LowerBorder + (Spread * u);
      }
      throw new InvalidOperationException(Validate());
    }

    private bool CheckTriangularCondition()
    {
      if ((_LowerBorder < _Mean) &&
          (_Mean < _UpperBorder))
      {
        return true;
      }
      return false;
    }

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (!CheckTriangularCondition())
      {
        messages.Append(
                        "Please check the paramter of the distribution. By a Triangular distribution it must apply for the parameteres: LowerBorder < Mean < UpperBorder ");
      }
      return messages;
    }
  }
}