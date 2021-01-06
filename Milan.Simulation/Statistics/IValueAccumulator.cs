#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Statistics
{
  public interface IValueAccumulator<TValue> : IValueTracker<TValue>
  {
    TValue Mean { get; }
    double MeanNumeric { get; }
    TValue StandardDeviation { get; }
    double StandardDeviationNumeric { get; }
    TValue Sum { get; }
    double SumNumeric { get; }
    TValue SquareSum { get; }
    double SquareSumNumeric { get; }
  }
}