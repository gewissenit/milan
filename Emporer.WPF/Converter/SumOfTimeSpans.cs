#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Emporer.WPF.Converter
{
  public class SumOfTimeSpans : AggregateOfTimeSpans
  {
    public override TimeSpan Aggregate(IEnumerable<TimeSpan> values)
    {
      return TimeSpan.FromTicks(values.Sum(v => v.Ticks));
    }
  }
}