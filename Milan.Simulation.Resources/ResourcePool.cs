#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Resources.Events;
using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ResourcePool : Entity, IResourcePool
  {
    private readonly IList<IResourceTypeAmount> _availableResourceTypeAmounts = new List<IResourceTypeAmount>();
    private readonly IDictionary<Resource, double> _recoveryStartTimes = new Dictionary<Resource, double>();
    private readonly IDictionary<Resource, int> _usagesForMaintenance = new Dictionary<Resource, int>();
    private readonly IList<Resource> _allResources = new List<Resource>();

    public IEnumerable<Resource> GetResources(IResourceType resourceType, int amount)
    {
      if (HasAvailable(resourceType, amount))
      {
        var resources = AvailableResources.Where(ar => ar.ResourceType == resourceType)
                                          .Take(amount)
                                          .ToArray();
        foreach (var resource in resources)
        {
          AvailableResources.Remove(resource);
          var timeSpan = CurrentExperiment.CurrentTime - _recoveryStartTimes[resource];
          resource.DecreaseInfluenceValues(timeSpan);
        }
        return resources;
      }
      throw new InvalidOperationException("This should not occur.");
    }

    public void Add(IResourceTypeAmount resourceAmount)
    {
      if (_availableResourceTypeAmounts.Any(cm => cm.ResourceType == resourceAmount.ResourceType))
      {
        throw new InvalidOperationException("An identical resource type already exists!");
      }
      _availableResourceTypeAmounts.Add(resourceAmount);
    }

    public void Remove(IResourceTypeAmount resourceAmount)
    {
      //todo: this should be done also by delete manager
      if (!_availableResourceTypeAmounts.Contains(resourceAmount))
      {
        throw new InvalidOperationException();
      }
      _availableResourceTypeAmounts.Remove(resourceAmount);
    }

    public event EventHandler ResourcesReceived;

    public bool HasAvailable(IResourceType resourceType, int amount)
    {
      return AvailableResources.Count(ar => ar.ResourceType == resourceType) >= amount;
    }

    [JsonProperty]
    public IEnumerable<IResourceTypeAmount> Resources
    {
      get { return _availableResourceTypeAmounts; }
    }

    public IList<Resource> AvailableResources { get; } = new List<Resource>();

    public IEnumerable<Resource> AllResources
    {
      get { return _allResources; }
    }

    public new void Initialize()
    {
      base.Initialize();

      foreach (var availableResource in AvailableResources)
      {
        if (availableResource.UsageAmount != null)
        {
          availableResource.CurrentUsageAmount = availableResource.UsageAmount.GetSample();
          _usagesForMaintenance.Add(availableResource, 0);
        }
        _recoveryStartTimes.Add(availableResource, 0);
        _allResources.Add(availableResource);
      }
    }

    public override void Reset()
    {
      _availableResourceTypeAmounts.Clear();
      AvailableResources.Clear();
      _usagesForMaintenance.Clear();
      base.Reset();
    }

    public void Return(Resource resource)
    {
      if (_usagesForMaintenance.ContainsKey(resource))
      {
        _usagesForMaintenance[resource]++;
      }

      if (_usagesForMaintenance.ContainsKey(resource) &&
          _usagesForMaintenance[resource] >= resource.CurrentUsageAmount)
      {
        var rms = new ResourceMaintenanceStartEvent(this, resource);
        rms.Schedule(0);
        var rme = new ResourceMaintenanceEndEvent(this, rms, resource);
        rme.Schedule(resource.MaintenanceDuration.GetSample());
        rme.OnOccur = _ =>
                      {
                        resource.CurrentUsageAmount = resource.UsageAmount.GetSample();
                        _usagesForMaintenance[resource] = 0;
                        AvailableResources.Add(resource);
                        _recoveryStartTimes[resource] = CurrentExperiment.CurrentTime;
                        if (!resource.IsAvailable)
                        {
                          
                        }
                        //todo: should this really be scheduled twice? see line 140
                        RaiseResourceReceived();
                      };
      }
      else
      {
        AvailableResources.Add(resource);
        _recoveryStartTimes[resource] = CurrentExperiment.CurrentTime;
      }


      RaiseResourceReceived();
    }

    private void RaiseResourceReceived()
    {
      ResourcesReceived?.Invoke(this, new EventArgs());
    }

    public IEnumerable<Resource> GetResources(IResourceTypeAmount resourceTypeAmount)
    {
      return GetResources(resourceTypeAmount.ResourceType, resourceTypeAmount.Amount);
    }
  }
}