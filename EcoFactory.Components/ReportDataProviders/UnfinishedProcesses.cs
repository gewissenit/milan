#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Observers;
using Milan.Simulation.Reporting;
using Milan.Simulation.Statistics;

namespace EcoFactory.Components.ReportDataProviders
{
  public class UnfinishedProcesses : ReportDataProvider
  {
    private IEnumerable<UnfinishedProcessStatistic> _processes;

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Unfinished Processes",
                 Description = @"",
                 ColumnHeaders = new[]
                                 {
                                   Constants.ExperimentHeaderName, Constants.StationHeaderName, Constants.ProcessHeaderName, "Product Type(s)", "Elapsed Duration", "Remaining Duration"
                                 },
                 Data = _processes.Select(p => new object[]
                                               {
                                                 p.ExperimentId, p.Entity, p.ProcessType, p.ProductType, p.ElapsedDuration, p.RemainingDuration
                                               })
                                  .ToArray()
               }
             };
    }

    protected override void Prepare()
    {
      _processes = AggregateUnfinishedProcesses<IWorkstation>(_source)
        .Concat(AggregateUnfinishedProcesses<IAssemblyStation>(_source))
        .Concat(AggregateUnfinishedProcesses<IProbabilityAssemblyStation>(_source))
        .Concat(AggregateUnfinishedProcesses<IParallelWorkstation>(_source))
        .Concat(AggregateUnfinishedProcesses<IInhomogeneousParallelWorkstation>(_source))
        .Concat(AggregateUnfinishedProcesses<IInhomogeneousWorkstation>(_source))
        .Concat(AggregateUnfinishedStorageProcesses(_source));
    }

    private IEnumerable<UnfinishedProcessStatistic> AggregateUnfinishedProcesses<TStationaryElement>(IEnumerable<IExperiment> batch) where TStationaryElement : IEntity
    {
      var unfinishedProcesses = batch.SelectMany(exp => exp.Model.Observers.OfType<ProcessObserver<TStationaryElement>>()
                                                           .Single()
                                                           .UnfinishedProcesses.Select(up => new UnfinishedProcessStatistic
                                                                                             {
                                                                                               ExperimentId = exp.Id.ToString(),
                                                                                               ProcessType = up.Process,
                                                                                               ProductType = up.ProductTypes.Any()
                                                                                                               ? up.ProductTypes.Aggregate((current, next) => $"{current}, {next}")
                                                                                                               : string.Empty,
                                                                                               Entity = up.Entity,
                                                                                               ElapsedDuration = up.ElapsedDuration.ToRealTimeSpan(),
                                                                                               RemainingDuration = up.RemainingDuration.ToRealTimeSpan()
                                                                                             }));
      return unfinishedProcesses;
    }

    private IEnumerable<UnfinishedProcessStatistic> AggregateUnfinishedStorageProcesses(IEnumerable<IExperiment> batch)
    {
      var unfinishedProcesses = batch.SelectMany(exp => exp.Model.Observers.OfType<StorageObserver>()
                                                           .Single()
                                                           .UnfinishedProcesses.Select(up => new UnfinishedProcessStatistic
                                                                                             {
                                                                                               ExperimentId = exp.Id.ToString(),
                                                                                               ProcessType = up.Process,
                                                                                               ProductType = up.ProductTypes.Any()
                                                                                                               ? up.ProductTypes.Aggregate((current, next) => $"{current}, {next}")
                                                                                                               : string.Empty,
                                                                                               Entity = up.Entity,
                                                                                               ElapsedDuration = up.ElapsedDuration.ToRealTimeSpan(),
                                                                                               RemainingDuration = up.RemainingDuration.ToRealTimeSpan()
                                                                                             }));
      return unfinishedProcesses;
    }
  }
}