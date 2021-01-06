#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;

namespace Milan.Simulation.Reporting
{
  /// <summary>
  ///   Provides input data for a report.
  /// </summary>
  public interface IReportDataProvider
  {
    bool IsPrepared { get; }
    IEnumerable<ReportDataSet> DataSets { get; }

    /// <summary>
    ///   Initializes the provided report input data using the specified raw data source.
    /// </summary>
    /// <param name="source">The source.</param>
    void Prepare(IBatch source);
  }
}