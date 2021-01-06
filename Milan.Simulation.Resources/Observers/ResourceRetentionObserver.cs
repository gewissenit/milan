#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Milan.Simulation.Resources.Events;
using Milan.Simulation.Resources.Statistics;

namespace Milan.Simulation.Resources.Observers
{
  public class ResourceRetentionObserver : EntityTypeObserver<IStationaryElement>
  {
    private readonly IDictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>> _residingProcessingResourceTypeAmountsPerProductTypeAndEntity = new Dictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>>();

    private readonly Dictionary<Resource, double> _residingResources = new Dictionary<Resource, double>();

    private readonly IDictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>> _residingSetupResourceTypeAmountsPerProductTypeAndEntity = new Dictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>>();

    private readonly ICollection<ResourceRetentionTimes> _resourceRetentionTimes = new List<ResourceRetentionTimes>();
    private readonly ICollection<ResourceTypeRetentionTimes> _resourceTypeAwaitingTimes = new List<ResourceTypeRetentionTimes>();
    private readonly ICollection<ResourceTypeRetentionTimes> _resourceTypeRetentionTimes = new List<ResourceTypeRetentionTimes>();

    public ResourceRetentionObserver()
    {
      Name = "Resource Retention Times";
    }

    public IEnumerable<ResourceTypeRetentionTimes> ResourceTypeRetentionTimes => _resourceTypeRetentionTimes;
    public IEnumerable<ResourceRetentionTimes> ResourceRetentionTimes => _resourceRetentionTimes;
    public IEnumerable<ResourceTypeRetentionTimes> ResourceTypeAwaitingTimeStatistic => _resourceTypeAwaitingTimes;

    public override void Reset()
    {
      _residingProcessingResourceTypeAmountsPerProductTypeAndEntity.Clear();
      _residingResources.Clear();
      _residingSetupResourceTypeAmountsPerProductTypeAndEntity.Clear();
      _resourceRetentionTimes.Clear();
      _resourceTypeAwaitingTimes.Clear();
      _resourceTypeRetentionTimes.Clear();
      base.Reset();
    }

    public override string ToString()
    {
      return $"Observer: Resource retention times (for {typeof(IEntity).Name})";
    }

    protected override void OnEntityTypeEventOccurred(ISimulationEvent e)
    {
      if (e.Sender == null)
      {
        return;
      }
      var processingRequest = e as ProcessingResourceRequestedEvent;
      if (processingRequest != null)
      {
        EnsureDictionaryHasKey(processingRequest, _residingProcessingResourceTypeAmountsPerProductTypeAndEntity);
        RememberRequestedResources(processingRequest, _residingProcessingResourceTypeAmountsPerProductTypeAndEntity);
      }
      var setupRequest = e as SetupResourceRequestedEvent;
      if (setupRequest != null)
      {
        EnsureDictionaryHasKey(setupRequest, _residingSetupResourceTypeAmountsPerProductTypeAndEntity);
        RememberRequestedResources(setupRequest, _residingProcessingResourceTypeAmountsPerProductTypeAndEntity);
      }
      var processingReceived = e as ProcessingResourceReceivedEvent;
      if (processingReceived != null)
      {
        UpdateStatisticsOnResourceReceived(processingReceived, _residingProcessingResourceTypeAmountsPerProductTypeAndEntity);
        RememberReceivedResources(processingReceived);
      }
      var setupReceived = e as SetupResourceReceivedEvent;
      if (setupReceived != null)
      {
        UpdateStatisticsOnResourceReceived(setupReceived, _residingSetupResourceTypeAmountsPerProductTypeAndEntity);
        RememberReceivedResources(setupReceived);
      }
      var released = e as ResourceReleasedEvent;
      if (released != null)
      {
        UpdateStatisticsOnResourceReleased(released);
      }
    }

    private static void EnsureDictionaryHasKey(ResourceRequestedEvent e, IDictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>> dictionary)
    {
      var entity = (IEntity) e.Sender;
      if (!dictionary.ContainsKey(entity))
      {
        dictionary.Add(entity, new Dictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>());
      }
      if (!dictionary[entity].ContainsKey(e.ProductType))
      {
        dictionary[entity].Add(e.ProductType, new Dictionary<IResourcePool, IDictionary<IResourceType, double>>());
      }
      foreach (var resourcePool in e.ResourceTypeAmounts.Keys)
      {
        if (!dictionary[entity][e.ProductType].ContainsKey(resourcePool))
        {
          dictionary[entity][e.ProductType].Add(resourcePool, new Dictionary<IResourceType, double>());
        }
        foreach (var resourceType in e.ResourceTypeAmounts[resourcePool].Keys)
        {
          if (!dictionary[(IEntity) e.Sender][e.ProductType][resourcePool].ContainsKey(resourceType))
          {
            dictionary[(IEntity) e.Sender][e.ProductType][resourcePool].Add(resourceType, 0);
          }
        }
      }
    }

    private void UpdateStatisticsOnResourceReleased(ResourceReleasedEvent resourceReleasedEvent)
    {
      var entity = (IEntity) resourceReleasedEvent.Sender;
      foreach (var resource in resourceReleasedEvent.Resources)
      {
        var receivedData = _residingResources[resource];
        var simulationRetentionTime = resourceReleasedEvent.ScheduledTime - receivedData;
        var retentionTime = simulationRetentionTime.ToRealTimeSpan();
        AddOrUpdateResourceTypeRetentionStatistics(entity.Name, resource.ResourceType.Name, resource.ResourcePool.Name, retentionTime);
        AddOrUpdateResourceRetentionStatistics(entity.Name, resource, retentionTime);

        _residingResources.Remove(resource);
      }
    }

    private void UpdateStatisticsOnResourceReceived(ResourceReceivedEvent e, IDictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>> dictionary)
    {
      var entity = (IEntity) e.Sender;
      foreach (var resourcePool in e.RequestedResourceTypeAmounts.Keys)
      {
        foreach (var resourceType in e.RequestedResourceTypeAmounts[resourcePool].Keys)
        {
          var receivedData = dictionary[entity][e.ProductType][resourcePool][resourceType];
          var retentionTime = (e.ScheduledTime - receivedData).ToRealTimeSpan();

          for (var i = 0; i < e.RequestedResourceTypeAmounts[resourcePool][resourceType]; i++)
          {
            AddOrUpdateResourceTypeAwaitingStatistics(entity.Name, resourceType.Name, resourcePool.Name, retentionTime);
          }
        }
      }
    }

    private void AddOrUpdateResourceTypeRetentionStatistics(string entity, string resourceType, string resourcePool, TimeSpan duration)
    {
      var match = _resourceTypeRetentionTimes.SingleOrDefault(rt => rt.Entity == entity && rt.ResourceType == resourceType && rt.ResourcePool == resourcePool);
      if (match == null)
      {
        match = new ResourceTypeRetentionTimes(entity, resourceType, resourcePool, () => CurrentExperiment.CurrentTime);
        _resourceTypeRetentionTimes.Add(match);
      }
      match.Values.Update(duration);
    }

    private void AddOrUpdateResourceRetentionStatistics(string entity, Resource resource, TimeSpan duration)
    {
      var match = _resourceRetentionTimes.SingleOrDefault(rt => rt.Entity == entity && rt.Resource == resource);
      if (match == null)
      {
        match = new ResourceRetentionTimes(entity, resource, () => CurrentExperiment.CurrentTime);
        _resourceRetentionTimes.Add(match);
      }
      match.Values.Update(duration);
    }

    private void AddOrUpdateResourceTypeAwaitingStatistics(string entity, string resourceType, string resourcePool, TimeSpan duration)
    {
      var match = _resourceTypeAwaitingTimes.SingleOrDefault(rt => rt.Entity == entity && rt.ResourceType == resourceType && rt.ResourcePool == resourcePool);
      if (match == null)
      {
        match = new ResourceTypeRetentionTimes(entity, resourceType, resourcePool, () => CurrentExperiment.CurrentTime);
        _resourceTypeAwaitingTimes.Add(match);
      }
      match.Values.Update(duration);
    }

    private static void RememberRequestedResources(ResourceRequestedEvent e, IDictionary<IEntity, IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, double>>>> dictionary)
    {
      foreach (var resourcePool in e.ResourceTypeAmounts.Keys)
      {
        foreach (var resourceType in e.ResourceTypeAmounts[resourcePool].Keys)
        {
          dictionary[(IEntity) e.Sender][e.ProductType][resourcePool][resourceType] = e.ScheduledTime;
        }
      }
    }

    private void RememberReceivedResources(ResourceReceivedEvent resourcesReceivedEvent)
    {
      foreach (var resource in resourcesReceivedEvent.Resources)
      {
        _residingResources.Add(resource, resourcesReceivedEvent.ScheduledTime);
      }
    }
  }
}