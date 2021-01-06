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
  public class StorageStates : ReportDataProvider
  {
    private IEnumerable<BatchProcessStatistic> _states;

    protected override void Prepare()
    {
      _states = AggregateStorageProcesses(_source);
    }

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Storage processes",
                 Description = @"",
                 ColumnHeaders = new[]
                                 {
                                   "Storage", "State", "ProductType", "MinCount", "AvgCount", "MaxCount", "MinDuration", "AvgDuration", "MaxDuration",
                                   "SumDuration"
                                 },
                 Data = AggregateRecords(_states)
                   .ToArray()
               }
             };
    }

    private IEnumerable<object[]> AggregateRecords(IEnumerable<BatchProcessStatistic> batchProcesses)
    {
      return batchProcesses.Select(p => new object[]
                                        {
                                          p.Entity, p.ProcessType, p.ProductType, p.MinCount, p.AvgCount, p.MaxCount, p.MinDuration, p.AvgDuration,
                                          p.MaxDuration, p.SumDuration
                                        });
    }

    private IEnumerable<BatchProcessStatistic> AggregateStorageProcesses(IBatch batch)
    {
      var storageProcesses = batch.SelectMany(exp => exp.Model.Observers.OfType<StorageObserver>()
                                                        .Single()
                                                        .States);
      var processes = AggregateStates(batch.Id, storageProcesses);
      return processes.ToArray();
    }


    private static IEnumerable<BatchProcessStatistic> AggregateStates(string batchId, IEnumerable<ProcessStatistic> states)
    {
      var aggregatedStates = from state in states
                             group state by new
                                            {
                                              state.Entity,
                                              state.Process,
                                              state.ProductType
                                            }
                             into distinctStates
                             select new BatchProcessStatistic(batchId)
                                    {
                                      Entity = distinctStates.Key.Entity,
                                      ProcessType = distinctStates.Key.Process,
                                      ProductType = distinctStates.Key.ProductType,
                                      SumDuration = (distinctStates.Average(p => p.Durations.Sum.Ticks)).TicksToTimeSpan(),
                                      MinDuration = (distinctStates.Average(p => p.Durations.Minimum.Ticks)).TicksToTimeSpan(),
                                      AvgDuration = (distinctStates.Average(p => p.Durations.Mean.Ticks)).TicksToTimeSpan(),
                                      MaxDuration = (distinctStates.Average(p => p.Durations.Maximum.Ticks)).TicksToTimeSpan(),
                                      MinCount = distinctStates.Min(p => p.Durations.Count),
                                      AvgCount = distinctStates.Average(p => p.Durations.Count),
                                      MaxCount = distinctStates.Max(p => p.Durations.Count),
                                      MinProcessedProductsPerProcess = distinctStates.Min(p => p.RelatedProducts.Mean),
                                      AvgProcessedProductsPerProcess = distinctStates.Average(p => p.RelatedProducts.Mean),
                                      MaxProcessedProductsPerProcess = distinctStates.Max(p => p.RelatedProducts.Mean),
                                      MinTotalProcessedProducts = distinctStates.Min(p => p.RelatedProducts.Count),
                                      AvgTotalProcessedProducts = distinctStates.Average(p => p.RelatedProducts.Count),
                                      MaxTotalProcessedProducts = distinctStates.Max(p => p.RelatedProducts.Count)
                                    };


      return aggregatedStates;
    }
  }
}