#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using FileHelpers;

namespace Emporer.Math.Distribution
{
  [DelimitedRecord(",")]
// ReSharper disable ClassNeverInstantiated.Global
  public class ListDistributionRecord
// ReSharper restore ClassNeverInstantiated.Global
  {
// ReSharper disable UnassignedField.Global
// ReSharper disable InconsistentNaming
    public double SecondsToNextOccurrence;
// ReSharper restore InconsistentNaming
// ReSharper restore UnassignedField.Global
  }
}