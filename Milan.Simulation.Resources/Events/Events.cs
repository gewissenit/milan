#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.Events;

namespace Milan.Simulation.Resources.Events
{
  //todo: create base classes for all events; streamline event properties
  public class ResourceReleasedEvent : RelatedEvent
  {
    private const string EventName = "Resource released";

    public ResourceReleasedEvent(IEntity sender, ISimulationEvent relatedStartEvent, IEnumerable<Resource> releasedResources)
      : base(sender, EventName, relatedStartEvent)
    {
      Resources = releasedResources;
    }

    public IEnumerable<Resource> Resources { get; private set; }
  }

  public abstract class ResourceReceivedEvent : RelatedEvent
  {
    private const string EventName = "Resource received";

    public ResourceReceivedEvent(IProductType productType, IEntity sender, ISimulationEvent relatedStartEvent, IEnumerable<Resource> receivedResources, IDictionary<IResourcePool, IDictionary<IResourceType, int>> requestedResourceTypeAmounts)
      : base(sender, EventName, relatedStartEvent)
    {
      Resources = receivedResources;
      RequestedResourceTypeAmounts = requestedResourceTypeAmounts;
      ProductType = productType;
    }

    public IEnumerable<Resource> Resources { get; private set; }
    public IDictionary<IResourcePool, IDictionary<IResourceType, int>> RequestedResourceTypeAmounts { get; private set; }
    public IProductType ProductType { get; private set; }
  }

  public abstract class ResourceRequestedEvent : SimulationEvent
  {
    private const string EventName = "Resource requested";

    public ResourceRequestedEvent(IProductType productType, IEntity sender, IDictionary<IResourcePool, IDictionary<IResourceType, int>> requestedResourceTypeAmounts)
      : base(sender, EventName)
    {
      ResourceTypeAmounts = requestedResourceTypeAmounts;
      ProductType = productType;
    }

    public IProductType ProductType { get; private set; }
    public IDictionary<IResourcePool, IDictionary<IResourceType, int>> ResourceTypeAmounts { get; private set; }
  }

  public class ProcessingResourceReceivedEvent : ResourceReceivedEvent
  {
    public ProcessingResourceReceivedEvent(IProductType productType, IEntity sender, ISimulationEvent relatedStartEvent, IEnumerable<Resource> receivedResources, IDictionary<IResourcePool, IDictionary<IResourceType, int>> requestedResourceTypeAmounts)
      : base(productType, sender, relatedStartEvent, receivedResources, requestedResourceTypeAmounts)
    {
    }
  }

  public class SetupResourceReceivedEvent : ResourceReceivedEvent
  {
    public SetupResourceReceivedEvent(IProductType productType, IEntity sender, ISimulationEvent relatedStartEvent, IEnumerable<Resource> receivedResources, IDictionary<IResourcePool, IDictionary<IResourceType, int>> requestedResourceTypeAmounts)
      : base(productType, sender, relatedStartEvent, receivedResources, requestedResourceTypeAmounts)
    {
    }
  }

  public class ProcessingResourceRequestedEvent : ResourceRequestedEvent
  {
    public ProcessingResourceRequestedEvent(IProductType productType, IEntity sender, IDictionary<IResourcePool, IDictionary<IResourceType, int>> requestedResourceTypeAmounts)
      : base(productType, sender, requestedResourceTypeAmounts)
    {
    }
  }

  public class SetupResourceRequestedEvent : ResourceRequestedEvent
  {
    public SetupResourceRequestedEvent(IProductType productType, IEntity sender, IDictionary<IResourcePool, IDictionary<IResourceType, int>> requestedResourceTypeAmounts)
      : base(productType, sender, requestedResourceTypeAmounts)
    {
    }
  }

  public class ResourceMaintenanceStartEvent : SimulationEvent
  {
    private const string EventName = "Resource Maintenance Start";

    public ResourceMaintenanceStartEvent(IEntity sender, Resource resource)
      : base(sender, EventName)
    {
      Resource = resource;
    }

    public Resource Resource { get; set; }
  }

  public class ResourceMaintenanceEndEvent : RelatedEvent
  {
    private const string EventName = "Resource Maintenance End";

    public ResourceMaintenanceEndEvent(IEntity sender, ISimulationEvent relatedStartEvent, Resource resource)
      : base(sender, EventName, relatedStartEvent)
    {
      Resource = resource;
    }

    public Resource Resource { get; set; }
  }
}