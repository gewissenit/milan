#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation;
using Milan.Simulation.Observers;
using Milan.Simulation.Reporting;

namespace EcoFactory.Components.ReportDataProviders
{
  public class Processes : ReportDataProvider
  {
    private IEnumerable<Position> _processes;

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Processes",
                 Description = @"",
                 ColumnHeaders = new[]
                                 {
                                   Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.ProductTypeHeaderName, Constants.DurationHeaderName, Constants.StartDateHeaderName, Constants.EndDateHeaderName, Constants.ProductHeaderName, Constants.ExperimentHeaderName, Constants.EventHeaderName
                                 },
                 Data = _processes.Select(bp =>
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
                                                Constants.ExperimentHeaderName, Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.ProductTypeHeaderName, Constants.ProductHeaderName
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
                                                Constants.ExperimentHeaderName, Constants.ProductTypeHeaderName, Constants.ProcessHeaderName, Constants.StationHeaderName, Constants.ProductHeaderName
                                              }
                            }
                          }
               }
             };
    }

    protected override void Prepare()
    {
      _processes = GetProcesses<IWorkstation>()
        .Concat(GetProcesses<IParallelWorkstation>())
        .Concat(GetProcesses<IInhomogeneousParallelWorkstation>())
        .Concat(GetProcesses<IInhomogeneousWorkstation>())
        .Concat(GetProcesses<IAssemblyStation>())
        .Concat(GetProcesses<IProbabilityAssemblyStation>());
    }

    private IEnumerable<Position> GetProcesses<TStationaryElement>() where TStationaryElement : IStationaryElement
    {
      return _source.SelectMany(e => e.Model.Observers.OfType<ProcessObserver<TStationaryElement>>()
                                      .SelectMany(o => o.FinishedPositions));
    }
  }
}