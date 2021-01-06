using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Reporting;
using Milan.Simulation.Resources.Observers;
using Milan.Simulation.Resources.Statistics;

namespace Milan.Simulation.Resources.ReportDataProviders
{
  public class ResourceTimes : ReportDataProvider
  {
    private IEnumerable<ResourceInfluence> _allResourceInfluences;
    private IEnumerable<ResourceRetentionTimes> _allResourceMaintenanceTimes;
    private IEnumerable<ResourceRetentionTimes> _allResourceRetentionTimes;
    private IEnumerable<ResourceTypeRetentionTimes> _allResourceTypeAmountRetentionTimes;
    private IEnumerable<ResourceTypeRetentionTimes> _allResourceTypeRetentionTimes;
    private IEnumerable<ResourceTypeRetentionTimeStatistic> _batchRetentionTimes;
    private IEnumerable<ResourceTypeRetentionTimeStatistic> _batchRetentionTimesByEntity;
    private IEnumerable<ResourceTypeRetentionTimeStatistic> _batchRetentionTimesByResourceType;
    private List<DetailedInfluenceDatum> _detailedInfluenceData;
    private IEnumerable<ResourceInfluenceStatistic> _resourceInfluences;
    private IEnumerable<ResourceRetentionTimeStatistic> _resourceMaintenanceTimes;
    private IEnumerable<ResourceRetentionTimeStatistic> _resourceRetentionTimes;
    private IEnumerable<ResourceRetentionTimeStatistic> _resourceRetentionTimesByStation;

    protected override void Prepare()
    {
      _allResourceTypeAmountRetentionTimes = _source.SelectMany(exp => exp.GetObserver<ResourceRetentionObserver>()
                                                                          .ResourceTypeAwaitingTimeStatistic);
      _batchRetentionTimes = from rt in _allResourceTypeAmountRetentionTimes
                             group rt by new
                                         {
                                           rt.Entity,
                                           rt.ResourceType
                                         }
                             into distinctStates
                             select new ResourceTypeRetentionTimeStatistic(distinctStates.Key.Entity, distinctStates.Key.ResourceType, distinctStates.Select(rt => rt.Values.Count), distinctStates.Select(rt => rt.Values.Minimum), distinctStates.Select(rt => rt.Values.Mean), distinctStates.Select(rt => rt.Values.Maximum), distinctStates.Select(rt => rt.Values.Sum));

      _allResourceTypeRetentionTimes = _source.SelectMany(exp => exp.GetObserver<ResourceRetentionObserver>()
                                                                    .ResourceTypeRetentionTimes)
                                              .ToArray();
      _batchRetentionTimesByEntity = from rt in _allResourceTypeRetentionTimes
                                      group rt by new
                                                  {
                                                    rt.Entity,
                                                    rt.ResourceType
                                                  }
                                      into distinctStates
                                      select new ResourceTypeRetentionTimeStatistic(distinctStates.Key.Entity, distinctStates.Key.ResourceType, distinctStates.Select(rt => rt.Values.Count), distinctStates.Select(rt => rt.Values.Minimum), distinctStates.Select(rt => rt.Values.Mean), distinctStates.Select(rt => rt.Values.Maximum), distinctStates.Select(rt => rt.Values.Sum));

      _batchRetentionTimesByResourceType = from rt in _allResourceTypeRetentionTimes
                                           group rt by new
                                                       {
                                                         rt.ResourceType,
                                                       }
                                           into distinctStates
                                           select new ResourceTypeRetentionTimeStatistic(null, distinctStates.Key.ResourceType, distinctStates.Select(rt => rt.Values.Count), distinctStates.Select(rt => rt.Values.Minimum), distinctStates.Select(rt => rt.Values.Mean), distinctStates.Select(rt => rt.Values.Maximum), distinctStates.Select(rt => rt.Values.Sum));

      _allResourceRetentionTimes = _source.SelectMany(exp => exp.GetObserver<ResourceRetentionObserver>()
                                                                .ResourceRetentionTimes)
                                          .ToArray();

      _resourceRetentionTimes = from rt in _allResourceRetentionTimes
                                group rt by new
                                            {
                                              rt.Resource
                                            }
                                into distinctStates
                                select new ResourceRetentionTimeStatistic(null, distinctStates.Key.Resource, distinctStates.Select(rt => rt.Values.Count), distinctStates.Select(rt => rt.Values.Minimum), distinctStates.Select(rt => rt.Values.Mean), distinctStates.Select(rt => rt.Values.Maximum), distinctStates.Select(rt => rt.Values.Sum));

      _detailedInfluenceData = new List<DetailedInfluenceDatum>();

      foreach (var experiment in _source.Select((exp, i) => new
                                                            {
                                                              Object = exp,
                                                              ExpNr = i
                                                            }))
      {
        foreach (var pool in experiment.Object.Model.Entities.OfType<IResourcePool>())
        {
          foreach (var resourceType in pool.AllResources.Select(r => r.ResourceType)
                                           .Distinct())
          {
            foreach (var resource in pool.AllResources.Where(r => r.ResourceType == resourceType)
                                         .Select((r, i) => new
                                                           {
                                                             Object = r,
                                                             Index = i
                                                           }))
            {
              foreach (var influence in resource.Object.InfluenceValues.Keys)
              {
                _detailedInfluenceData.Add(new DetailedInfluenceDatum()
                                           {
                                             Experiment = experiment.ExpNr + 1,
                                             ResourcePool = pool.Name,
                                             ResourceType = resourceType.Name,
                                             Resource = $"{resourceType.Name} {resource.Index + 1}",
                                             Influence = influence.Name,
                                             ValuesOverTime = resource.Object.InfluenceValues[influence].ValuesOverTime.Select(x => new ValueAtPointInTime()
                                                                                                                                    {
                                                                                                                                      PointInTime = x.PointInTime,
                                                                                                                                      Value = x.Value
                                                                                                                                    })
                                                                      .ToArray()
                                           });
              }
            }
          }
        }
      }


      var items = _source.SelectMany(exp => exp.Model.Entities.OfType<IResourcePool>()
                                               .SelectMany(rp => rp.AllResources));

      _allResourceInfluences = items.SelectMany(r => r.InfluenceValues.Select(i => new ResourceInfluence
                                                                                   {
                                                                                     Resource = r,
                                                                                     Influence = i.Key,
                                                                                     Value = i.Value
                                                                                   }));

      _resourceInfluences = from rt in _allResourceInfluences
                            group rt by new
                                        {
                                          rt.Influence,
                                          rt.Resource
                                        }
                            into distinctStates
                            select new ResourceInfluenceStatistic(distinctStates.Key.Resource, distinctStates.Key.Influence, distinctStates.Select(rt => rt.Value.Count), distinctStates.Select(rt => rt.Value.Minimum), distinctStates.Select(rt => rt.Value.Mean), distinctStates.Select(rt => rt.Value.Maximum), distinctStates.Select(rt => rt.Value.Sum));

      _allResourceMaintenanceTimes = _source.SelectMany(exp => exp.GetObserver<ResourceMaintenanceObserver>()
                                                                  .ResourceMaintenanceTimes)
                                            .ToArray();

      _resourceMaintenanceTimes = from rt in _allResourceMaintenanceTimes
                                  group rt by new
                                              {
                                                rt.Resource
                                              }
                                  into distinctStates
                                  select new ResourceRetentionTimeStatistic(null, distinctStates.Key.Resource, distinctStates.Select(rt => rt.Values.Count), distinctStates.Select(rt => rt.Values.Minimum), distinctStates.Select(rt => rt.Values.Mean), distinctStates.Select(rt => rt.Values.Maximum), distinctStates.Select(rt => rt.Values.Sum));

      _resourceRetentionTimesByStation = from rt in _allResourceRetentionTimes
                                         group rt by new
                                                     {
                                                       rt.Resource,
                                                       rt.Entity
                                                     }
                                         into distinctStates
                                         select new ResourceRetentionTimeStatistic(distinctStates.Key.Entity, distinctStates.Key.Resource, distinctStates.Select(rt => rt.Values.Count), distinctStates.Select(rt => rt.Values.Minimum), distinctStates.Select(rt => rt.Values.Mean), distinctStates.Select(rt => rt.Values.Maximum), distinctStates.Select(rt => rt.Values.Sum));
    }

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      var resourceInfluences = CreateResourceInfluenceReportDataSet();
      var retentionTimesPerResourceTypePerEntity = CreateResourceTypePerStationRetentionTimeReportDataSet();
      var retentionTimesPerResourceType = CreateResourceTypeRetentionTimeReportDataSet();
      var retentionTimesPerResource = CreateResourceRetentionTimeReportDataSet();
      var retentionTimesPerResourcePerStation = CreateResourceRetentionTimePerStationReportDataSet();
      var awaitingTimesPerResourceTypePerStation = CreateResourceTypeAwaitingReportDataSet();
      var maintenanceTimesPerResource = CreateResourceMaintenanceTimeReportDataSet();

      return new[]
             {
               retentionTimesPerResourceTypePerEntity, retentionTimesPerResourceType, retentionTimesPerResource, retentionTimesPerResourcePerStation, awaitingTimesPerResourceTypePerStation, maintenanceTimesPerResource, resourceInfluences, CreateDetailedInfluenceData()
             };
    }

    private ReportDataSet CreateResourceTypePerStationRetentionTimeReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "ResourceType retention Times per Entity",
               Description = "Shows how long a Station used a specific Resource and how often",
               ColumnHeaders = new[]
                               {
                                 "Station", "ResourceType", "Count", "MinDuration", "AvgDuration", "MaxDuration", "SumDuration"
                               },
               Data = _batchRetentionTimesByEntity.Select(rt => new object[]
                                                                 {
                                                                   rt.Entity, rt.ResourceType, rt.MinCount, rt.MinDuration, rt.AvgDuration, rt.MaxDuration, rt.SumDuration
                                                                 })
                                                   .ToArray()
             };
    }

    private ReportDataSet CreateDetailedInfluenceData()
    {
      return new ReportDataSet
             {
               Name = "Detailed Influence Data",
               Description = "",
               ColumnHeaders = new[]
                               {
                                 "Experiment", "Pool", "Type", "Resource", "Influence", "Values over time >"
                               },
               Data = _detailedInfluenceData.SelectMany(CreateTwoLinesForEveryEntry)
                                            .ToArray()
             };
    }

    private object[][] CreateTwoLinesForEveryEntry(DetailedInfluenceDatum x)
    {
      var line1 = new object[]
                  {
                    x.Experiment, x.ResourcePool, x.ResourceType, x.Resource, x.Influence,
                  }.Concat(x.ValuesOverTime.Select(v => v.PointInTime)
                            .Cast<object>())
                   .ToArray();
      var line2 = new object[]
                  {
                    "", "", "", "", "Values:"
                  }.Concat(x.ValuesOverTime.Select(v => v.Value)
                            .Cast<object>())
                   .ToArray();

      return new object[][]
             {
               line1, line2
             };
    }

    private ReportDataSet CreateResourceTypeAwaitingReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "Resource waiting",
               Description = "Shows how long a Station waited for a Resource and how often",
               ColumnHeaders = new[]
                               {
                                 "Station", "ResourceType", "Count", "MinDuration", "AvgDuration", "MaxDuration", "SumDuration"
                               },
               Data = _batchRetentionTimes.Select(rt => new object[]
                                                        {
                                                          rt.Entity, rt.ResourceType, rt.MaxCount, rt.MinDuration, rt.AvgDuration, rt.MaxDuration, rt.SumDuration
                                                        })
                                          .ToArray()
             };
    }

    private ReportDataSet CreateResourceTypeRetentionTimeReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "RT retention Times per ResourceType",
               Description = "Shows how long a ResourceType was used and how often",
               ColumnHeaders = new[]
                               {
                                 "ResourceType", "Count", "MinDuration", "AvgDuration", "MaxDuration", "SumDuration"
                               },
               Data = _batchRetentionTimesByResourceType.Select(rt => new object[]
                                                                      {
                                                                        rt.ResourceType, rt.SumCounts, rt.MinDuration, rt.AvgDuration, rt.MaxDuration, rt.SumDuration
                                                                      })
                                                        .ToArray()
             };
    }

    private ReportDataSet CreateResourceRetentionTimeReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "Resource retention Times",
               Description = "Shows how long a Resource was used and how often",
               ColumnHeaders = new[]
                               {
                                 "Resource", "ResourceType", "Count", "MinDuration", "AvgDuration", "MaxDuration", "SumDuration"
                               },
               Data = _resourceRetentionTimes.Select(rt => new object[]
                                                           {
                                                             rt.Resource.ToString(), rt.Resource.ResourceType.Name, rt.SumCounts, rt.MinDuration, rt.AvgDuration, rt.MaxDuration, rt.SumDuration
                                                           })
                                             .ToArray()
             };
    }

    private ReportDataSet CreateResourceInfluenceReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "Resource influences",
               Description = "to do",
               ColumnHeaders = new[]
                               {
                                 "ResourceType", "Resource", "Influence", "Count", "Min", "Avg", "Max", "Sum"
                               },
               Data = _resourceInfluences.Select(rt => new object[]
                                                       {
                                                         rt.Resource.ResourceType.Name, rt.Resource.ToString(), rt.Influence.Name, rt.SumCounts, rt.Min, rt.Avg, rt.Max, rt.Sum
                                                       })
                                         .ToArray()
             };
    }

    private ReportDataSet CreateResourceMaintenanceTimeReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "Resource maintenance times",
               Description = "Shows how long a Resource was maintained and how often",
               ColumnHeaders = new[]
                               {
                                 "Resource", "ResourceType", "Count", "MinDuration", "AvgDuration", "MaxDuration", "SumDuration"
                               },
               Data = _resourceMaintenanceTimes.Select(rt => new object[]
                                                             {
                                                               rt.Resource.ToString(), rt.Resource.ResourceType.Name, rt.SumCounts, rt.MinDuration, rt.AvgDuration, rt.MaxDuration, rt.SumDuration
                                                             })
                                               .ToArray()
             };
    }

    private ReportDataSet CreateResourceRetentionTimePerStationReportDataSet()
    {
      return new ReportDataSet
             {
               Name = "Resource retention Times per Station",
               Description = "Shows how long a Resource was used and how often",
               ColumnHeaders = new[]
                               {
                                 "Resource", "ResourceType", "Station", "Count", "MinDuration", "AvgDuration", "MaxDuration", "SumDuration"
                               },
               Data = _resourceRetentionTimesByStation.Select(rt => new object[]
                                                                    {
                                                                      rt.Resource.ToString(), rt.Resource.ResourceType, rt.Entity, rt.SumCounts, rt.MinDuration, rt.AvgDuration, rt.MaxDuration, rt.SumDuration
                                                                    })
                                                      .ToArray()
             };
    }
  }
}