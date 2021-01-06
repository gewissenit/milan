#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.States;
using Milan.Simulation;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Events;
using GeWISSEN.Utils;

namespace EcoFactory.Components
{
  public class ResourceManager
  {
    private readonly IState _awaitingResourcesEvent;
    private readonly Action _enterProductionAction;

    public ResourceManager(IState awaitingResourcesEvent, Action enterProductionAction)
    {
      Throw.IfNull(awaitingResourcesEvent, "awaitingResourcesEvent");
      Throw.IfNull(enterProductionAction, "enterProductionAction");

      _awaitingResourcesEvent = awaitingResourcesEvent;
      _enterProductionAction = enterProductionAction;
    }

    public bool ResourcesAvailable { get; private set; }

    public IList<Resource> AvailableResources { get; } = new List<Resource>();

    public IDictionary<IResourcePool, IDictionary<IResourceType, int>> RequestedResources { get; set; }

    public void HandleResources()
    {
      ResourcesAvailable = false;
      if (!RequestedResources.Any())
      {
        return; // Misconfigured. Will never be available.
      }
      _awaitingResourcesEvent.Enter();
      if (TryGetResources())
      {
        _awaitingResourcesEvent.Exit();
        ResourcesAvailable = true;
      }
      else
      {
        RegisterOnResourcesReceived();
      }
    }

    private void RegisterOnResourcesReceived()
    {
      foreach (var resourcePool in RequestedResources.Keys)
      {
        resourcePool.ResourcesReceived += OnResourcesReceived;
      }
    }

    private void UnregisterOnResourcesReceived()
    {
      foreach (var resourcePool in RequestedResources.Keys)
      {
        resourcePool.ResourcesReceived -= OnResourcesReceived;
      }
    }

    public void EnterProductionProcess()
    {
      if (ResourcesAvailable)
      {
        _enterProductionAction();
      }
    }

    public void HandleResourcesAndEnterProductionProcess()
    {
      if (RequestedResources.Any())
      {
        if (!_awaitingResourcesEvent.Active)
        {
          _awaitingResourcesEvent.Enter();
        }
        if (TryGetResources())
        {
          _awaitingResourcesEvent.Exit();
          _enterProductionAction();
        }
        else
        {
          RegisterOnResourcesReceivedEnterProcess();
        }
      }
      else
      {
        _enterProductionAction();
      }
    }

    private void RegisterOnResourcesReceivedEnterProcess()
    {
      foreach (var resourcePool in RequestedResources.Keys)
      {
        resourcePool.ResourcesReceived += OnResourcesReceivedEnterProcess;
      }
    }

    private void UnregisterOnResourcesReceivedEnterProcess()
    {
      foreach (var resourcePool in RequestedResources.Keys)
      {
        resourcePool.ResourcesReceived -= OnResourcesReceivedEnterProcess;
      }
    }

    private void OnResourcesReceivedEnterProcess(object sender, EventArgs eventArgs)
    {
      if (!TryGetResources())
      {
        return;
      }
      UnregisterOnResourcesReceivedEnterProcess();

      _awaitingResourcesEvent.Exit();
      _enterProductionAction();
    }

    private void OnResourcesReceived(object sender, EventArgs eventArgs)
    {
      if (!TryGetResources())
      {
        return;
      }
      UnregisterOnResourcesReceived();

      _awaitingResourcesEvent.Exit();
      ResourcesAvailable = true;
    }

    private bool TryGetResources()
    {
      if (!RequestedResources.All(p => p.Value.All(r => p.Key.HasAvailable(r.Key, r.Value))))
      {
        return false;
      }
      foreach (var resources in RequestedResources.Keys.Select(pool => RequestedResources[pool].SelectMany(r => pool.GetResources(r.Key, r.Value))))
      {
        resources.ForEach(AvailableResources.Add);
      }
      return true;
    }

    public void ReturnResources(IEntity entity, ResourceReceivedEvent lastResourceReceivedEvent)
    {
      if (!AvailableResources.Any())
      {
        return;
      }
      
      var resourcesReleased = new ResourceReleasedEvent(entity, lastResourceReceivedEvent, AvailableResources.ToArray());
      resourcesReleased.Schedule(0);

      foreach (var resource in AvailableResources)
      {
        var influenceAware = entity as IInfluenceAware;
        if (influenceAware != null)
        {
          foreach (var influenceRate in influenceAware.Influences)
          {
            resource.IncreaseInfluenceValue(influenceRate.Influence, influenceRate.Value, resourcesReleased.Duration);
          }
        }
        resource.ResourcePool.Return(resource);
      }

      AvailableResources.Clear();
    }
  }
}