#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;

namespace Milan.Simulation.Statistics
{
  public class Histogram<TValue> : ValueAccumulator<TValue>
  {
    private readonly Dictionary<double, int> _values = new Dictionary<double, int>();

    public Histogram(int cells)
    {
      Cells = cells < 1
                ? 1
                : cells;
    }

    public double Width
    {
      get { return ToDouble(Maximum) - ToDouble(Minimum) / Cells; }
    }

    public int Cells { get; set; }

    private Dictionary<double, int> Values
    {
      get { return _values; }
    }

    public double GetLowerLimitForCell(int cell)
    {
      if (cell < 0 ||
          cell > Cells + 1)
      {
        throw new ArgumentException();
      }
      return ToDouble(Minimum) + cell * Width;
    }

    public int GetMostFrequentedCell()
    {
      var max = int.MinValue;
      var cellValue = ToDouble(Minimum);
      foreach (var pair in Values)
      {
        if (max >= pair.Value)
        {
          continue;
        }
        max = pair.Value;
        cellValue = pair.Key;
      }
      return GetCellForValue(cellValue);
    }

    public int GetCellForValue(double value)
    {
      return (int) Math.Floor((value - ToDouble(Minimum)) / Width);
    }

    public override void Update(TValue value)
    {
      base.Update(value);

      var numericValue = ToDouble(value);

      if (Values.ContainsKey(numericValue))
      {
        Values[numericValue]++;
      }
      else
      {
        Values.Add(numericValue, 1);
      }
    }
  }
}