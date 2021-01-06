#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Observers;
using Milan.Simulation;
using Milan.Simulation.Reporting;
using Milan.Simulation.Statistics;

namespace EcoFactory.Components.ReportDataProviders
{
  public class StorageFillLevels : ReportDataProvider
  {
    private IEnumerable<BatchFillLevelStatistic> _fillLevels;

    protected override void Prepare()
    {
      _fillLevels = AggregateStorageFillLevels(_source);
    }

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Storage fill levels",
                 Description = "see title, better description will follow.",
                 ColumnHeaders = new[]
                                 {
                                   "Storage", "ProductType", "Capacity", "TimeEmpty", "TimeInUse", "TimeFull", "Min", "Mean", "Max"
                                 },
                 Data = _fillLevels.Select(fl => new object[]
                                                 {
                                                   fl.Entity, fl.ProductType, fl.Capacity, fl.TimeEmpty, fl.TimeInUse, fl.TimeFull, fl.Minimum,
                                                   fl.Mean, fl.Maximum
                                                 })
                                   .ToArray()
               }
             };
    }
    
    private IEnumerable<FillLevelStatistic> GetFillLevelStatisticsFromObserver(IExperiment experiment)
    {
      return experiment.Model.Observers.OfType<StorageObserver>()
                       .Single()
                       .FillLevels;
    }

    private IEnumerable<BatchFillLevelStatistic> AggregateStorageFillLevels(IEnumerable<IExperiment> batch)
    {
      var fillLevelStatisticsFromAllExperiments = batch.SelectMany(GetFillLevelStatisticsFromObserver)
                                                       .ToArray();
      var aggregateStorageFillLevels = from statistic in fillLevelStatisticsFromAllExperiments
                                       group statistic by new
                                                          {
                                                            Entity = statistic.Storage,
                                                            statistic.ProductType
                                                          }
                                       into distinctStates
                                       select
                                         new BatchFillLevelStatistic(distinctStates.Key.Entity,
                                                                     distinctStates.Key.ProductType,
                                                                     distinctStates.Select(d => d.Values),
                                                                     distinctStates.First()
                                                                                   .Capacity);

      return aggregateStorageFillLevels;
    }
  }
}