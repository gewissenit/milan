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
  public class ResourceMaintenanceObserver : EntityTypeObserver<IResourcePool>
  {
    private readonly ICollection<ResourceRetentionTimes> _resourceMaintenanceTimes = new List<ResourceRetentionTimes>();

    public ResourceMaintenanceObserver()
    {
      Name = "Resource Maintenance Times";
    }

    public IEnumerable<ResourceRetentionTimes> ResourceMaintenanceTimes
    {
      get
      {
        return _resourceMaintenanceTimes;
      }
    }
    
    public override string ToString()
    {
      return string.Format("Observer: Resource maintenance times (for {0})", typeof (IEntity).Name);
    }

    public override void Reset()
    {
      _resourceMaintenanceTimes.Clear();
      base.Reset();
    }

    protected override void OnEntityTypeEventOccurred(ISimulationEvent e)
    {
      if (e.Sender == null)
      {
        return;
      }
      var resourceMaintenanceEndEvent = e as ResourceMaintenanceEndEvent;
      if (resourceMaintenanceEndEvent != null)
      {
        AddOrUpdateResourceMaintenanceStatistics(((IEntity) e.Sender).Name, resourceMaintenanceEndEvent.Resource, resourceMaintenanceEndEvent.Duration.ToRealTimeSpan());
      }
    }

    private void AddOrUpdateResourceMaintenanceStatistics(string entity, Resource resource, TimeSpan duration)
    {
      var match = _resourceMaintenanceTimes.SingleOrDefault(rt => rt.Entity == entity && rt.Resource == resource);
      if (match == null)
      {
        match = new ResourceRetentionTimes(entity, resource, () => CurrentExperiment.CurrentTime);
        _resourceMaintenanceTimes.Add(match);
      }
      match.Values.Update(duration);
    }
  }
}