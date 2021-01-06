#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Reporting
{
  public class Times : ReportDataProvider
  {
    private DateTime _batchFinishedDate;
    private TimeSpan _batchRunTime;
    // real times
    private DateTime _batchStartedDate;
    private TimeSpan _experimentRunTimeAvg;
    private TimeSpan _experimentRunTimeMax;
    private TimeSpan _experimentRunTimeMin;
    private TimeSpan _simulatedDurationAvg;
    private TimeSpan _simulatedDurationMax;
    private TimeSpan _simulatedDurationMin;

    // simulated times
    private DateTime _simulatedStartDate;


    protected override void Prepare()
    {
      _batchStartedDate = _source.BatchStartedDate;
      _batchFinishedDate = _source.BatchStartedDate + _source.BatchRunTime;
      _batchRunTime = _source.BatchRunTime;

      if (!_source.Any())
      {
        ReportAggregationFailure();
        return; // something went wrong, no experiments available
      }

      var experimentObservers = _source.Select(exp => exp.Model.Observers.OfType<ExperimentObserver>()
                                                         .First())
                                       .ToArray();

      if (experimentObservers.Count() != _source.Count())
      {
        ReportAggregationFailure();
        return;
      }

      var simulationTimes = experimentObservers.Select(o => o.ExperimentRunTime)
                                               .ToArray();

      _experimentRunTimeMin = simulationTimes.Min();
      _experimentRunTimeMax = simulationTimes.Max();
      _experimentRunTimeAvg = TimeSpan.FromTicks(Convert.ToInt64(Math.Round(simulationTimes.Select(t => t.Ticks)
                                                                                           .Average())));

      var anObserver = experimentObservers.First();
      _simulatedStartDate = anObserver.SimulatedStartDate;

      _simulatedDurationMin = TimeSpan.FromTicks(Convert.ToInt64(experimentObservers.Min(o => o.SimulatedDuration.Ticks)));
      _simulatedDurationAvg = TimeSpan.FromTicks(Convert.ToInt64(experimentObservers.Average(o => o.SimulatedDuration.Ticks)));
      _simulatedDurationMax = TimeSpan.FromTicks(Convert.ToInt64(experimentObservers.Max(o => o.SimulatedDuration.Ticks)));
    }

    private void ReportAggregationFailure()
    {
      //TODO: mark results as failed (export should notify the user about missing data, ...)
    }

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      var events = new ReportDataSet
                   {
                     Name = "Times",
                     Description = @"",
                     ColumnHeaders = new[]
                                     {
                                       "Property", "Value"
                                     },
                     Data = new[]
                            {
                              new object[]
                              {
                                "Batch started (real time)", _batchStartedDate
                              },
                              new object[]
                              {
                                "Batch finished (real time)", _batchFinishedDate
                              },
                              new object[]
                              {
                                "Batch run time (real time)", _batchRunTime
                              },
                              new object[]
                              {
                                "Min experiment run time (real time)", _experimentRunTimeMin
                              },
                              new object[]
                              {
                                "Avg experiment run time (real time)", _experimentRunTimeAvg
                              },
                              new object[]
                              {
                                "Max experiment run time (real time)", _experimentRunTimeMax
                              },
                              new object[]
                              {
                                "Simulated start date", _simulatedStartDate
                              },
                              new object[]
                              {
                                "Min simulated time", _simulatedDurationMin
                              },
                              new object[]
                              {
                                "Avg simulated time", _simulatedDurationAvg
                              },
                              new object[]
                              {
                                "Max simulated time", _simulatedDurationMax
                              }
                            }
                   };

      return new[]
             {
               events
             };
    }
  }
}