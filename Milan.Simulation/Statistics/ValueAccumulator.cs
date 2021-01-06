#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class ValueAccumulator<TValue> : ValueTracker<TValue>, IValueAccumulator<TValue>
  {
    public ValueAccumulator()
    {
      SumNumeric = 0;
      SquareSumNumeric = 0;
    }

    public ValueAccumulator(Func<TValue, double> toDouble, Func<double, TValue> fromDouble)
      : base(toDouble, fromDouble)
    {
    }

    public virtual double SumNumeric { get; protected set; }

    public virtual double MeanNumeric
    {
      get { return CalculateMean(); }
    }

    public virtual double SquareSumNumeric { get; protected set; }

    public virtual double StandardDeviationNumeric
    {
      get { return CalculateStandardDeviation(); }
    }

    public virtual TValue Mean
    {
      get { return FromDouble(MeanNumeric); }
    }

    public virtual TValue Sum
    {
      get { return FromDouble(SumNumeric); }
    }

    public virtual TValue SquareSum
    {
      get { return FromDouble(SquareSumNumeric); }
    }

    public virtual TValue StandardDeviation
    {
      get { return FromDouble(StandardDeviationNumeric); }
    }

    public override void Update(TValue value)
    {
      base.Update(value);
      var currentValue = ToDouble(CurrentValue);
      SumNumeric += currentValue;
      SquareSumNumeric += currentValue * currentValue;
    }


    private double CalculateMean()
    {
      if (Count == 0)
      {
        return -1;
      }
      return SumNumeric / Count;
    }

    private double CalculateStandardDeviation()
    {
      if (Count == 0)
      {
        return -1;
      }
      if (Count == 1)
      {
        return 0;
      }
      return Math.Sqrt(Math.Abs(Count * SquareSumNumeric - SumNumeric * SumNumeric) / (Count * (Count - 1)));
    }

    public override string ToString()
    {
      return string.Format("Cnt:{0} Avg:{1} Sum:{2}", Count, Mean, Sum);
    }
  }
}