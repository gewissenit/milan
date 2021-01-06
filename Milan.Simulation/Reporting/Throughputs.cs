#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Reporting
{
  public class Throughputs : ReportDataProvider
  {
    private IEnumerable<Position> _throughputPositions;

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Throughputs",
                 Description = @"",
                 ColumnHeaders = new[]
                                 {
                                   Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.ProductTypeHeaderName, Constants.DurationHeaderName, Constants.StartDateHeaderName, Constants.EndDateHeaderName, Constants.ProductHeaderName, Constants.ExperimentHeaderName, Constants.EventHeaderName
                                 },
                 Data = _throughputPositions.Select(bp =>
                                                    {
                                                      var ptName = bp.ProductType == null
                                                                     ? ""
                                                                     : bp.ProductType.Name;
                                                      return new object[]
                                                             {
                                                               bp.Station.Name, bp.Process, ptName, bp.Duration, bp.StartDate, bp.EndDate, bp.Product, bp.Experiment.Id, bp.Event
                                                             };
                                                    })
                                            .ToArray(),
                 Pivots = new[]
                          {
                            new Pivot
                            {
                              Name = "Stations",
                              StartRow = 3,
                              NoGrandTotals = true,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.MinHeaderName,
                                               Format = Constants.TimeSpanFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.AvgHeaderName,
                                               Format = Constants.TimeSpanFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.MaxHeaderName,
                                               Format = Constants.TimeSpanFormat,
                                               Function = DataField.Functions.Max
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.CountHeaderName,
                                               Format = Constants.IntFormat,
                                               Function = DataField.Functions.Count
                                             }
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.ExperimentHeaderName, Constants.ProcessHeaderName, Constants.StationHeaderName, Constants.ProductTypeHeaderName, Constants.ProductHeaderName
                                              }
                            },
                            new Pivot
                            {
                              Name = "Products",
                              StartRow = 3,
                              NoGrandTotals = true,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.MinHeaderName,
                                               Format = Constants.TimeSpanFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.AvgHeaderName,
                                               Format = Constants.TimeSpanFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.MaxHeaderName,
                                               Format = Constants.TimeSpanFormat,
                                               Function = DataField.Functions.Max
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = Constants.CountHeaderName,
                                               Format = Constants.IntFormat,
                                               Function = DataField.Functions.Count
                                             }
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.ExperimentHeaderName, Constants.ProcessHeaderName, Constants.ProductTypeHeaderName, Constants.StationHeaderName, Constants.ProductHeaderName
                                              }
                            },
                            new Pivot
                            {
                              Name = "Pathways",
                              StartRow = 3,
                              NoGrandTotals = true,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.DurationHeaderName,
                                               Name = "Pathways",
                                               Format = Constants.TimeSpanFormat
                                             }
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.ExperimentHeaderName, Constants.ProductHeaderName, Constants.EndDateHeaderName, Constants.StationHeaderName
                                              }
                            }
                          }
               }
             };
    }

    protected override void Prepare()
    {
      _throughputPositions = _source.SelectMany(e => e.Model.Observers.OfType<ThroughputObserver>()
                                                      .SelectMany(o => o.StatisticPositions));
    }
  }
}