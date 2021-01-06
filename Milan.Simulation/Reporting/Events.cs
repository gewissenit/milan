#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Reporting
{
  public class Events : ReportDataProvider
  {
    private IEnumerable<Position> _eventPositions;

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new List<ReportDataSet>();
      //return new[]
      //       {
      //         new ReportDataSet
      //         {
      //           Name = "Events",
      //           Description = @"",
      //           ColumnHeaders = new[]
      //                           {
      //                             Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.OccuredDateHeaderName, Constants.ExperimentHeaderName, Constants.EventHeaderName
      //                           },
      //           Data = _eventPositions.Select(bp =>
      //                                         {
      //                                           var stName = bp.Station == null
      //                                                          ? ""
      //                                                          : bp.Station.Name;
      //                                           return new object[]
      //                                                  {
      //                                                    stName, bp.Process, bp.EndDate, bp.Experiment.Id, bp.Event
      //                                                  };
      //                                         })
      //                                 .ToArray(),
      //           Pivots = new[]
      //                    {
      //                      new Pivot
      //                      {
      //                        Name = "Count",
      //                        StartRow = 3,
      //                        DataFields = new[]
      //                                     {
      //                                       new DataField
      //                                       {
      //                                         SourceName = Constants.EventHeaderName,
      //                                         Name = Constants.EventHeaderName,
      //                                         Format = Constants.IntFormat,
      //                                         Function = DataField.Functions.Count
      //                                       }
      //                                     },
      //                        RowFieldNames = new[]
      //                                        {
      //                                          Constants.ExperimentHeaderName, Constants.ProcessHeaderName, Constants.StationHeaderName
      //                                        }
      //                      }
      //                    }
      //         }
      //       };
    }

    protected override void Prepare()
    {
      _eventPositions = _source.SelectMany(e => e.Model.Observers.OfType<ExperimentObserver>()
                                                 .SelectMany(o => o.StatisticPositions));
    }
  }
}