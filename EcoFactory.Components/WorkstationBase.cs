#region License

// Copyright (c) 2013 HTW Berlin All rights reserved.

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Events;
using EcoFactory.Components.States;
using Emporer.Math.Distribution;
using GeWISSEN.Utils;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Events;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public abstract class WorkstationBase : StationaryElement, IWorkstationBase
  {
    [JsonProperty]
    private readonly IList<IProductTypeDistribution> _BatchSizes = new List<IProductTypeDistribution>();

    [JsonProperty]
    private readonly IList<IInfluenceRate> _influences = new List<IInfluenceRate>();

    [JsonProperty]
    private readonly IList<IProductTypeDistribution> _ProcessingDurations = new List<IProductTypeDistribution>();

    [JsonProperty]
    private readonly IList<IResourcePoolResourceTypeAmount> _processingResources = new List<IResourcePoolResourceTypeAmount>();

    [JsonProperty]
    private readonly IList<IProductTypeSpecificResource> _productTypeSpecificProcessingResources = new List<IProductTypeSpecificResource>();

    [JsonProperty]
    private readonly IList<IProductTypeDistribution> _SetupDurations = new List<IProductTypeDistribution>();

    [JsonProperty]
    private readonly IList<IResourcePoolResourceTypeAmount> _SetupResources = new List<IResourcePoolResourceTypeAmount>();

    protected Queue<Product> _arrivedProducts;
    protected Infinite _awaitingProcessingResources;
    protected Infinite _awaitingSetupResources;
    private IDictionary<IProductType, IRealDistribution> _batchesDictionary;
    protected Infinite _blocked;
    protected Queue<Product> _blockedProducts;
    protected double? _currentBatchSize;
    protected RecurringTemporal _failure;
    protected Infinite _idle;
    protected ResourceReceivedEvent _lastProcessingResourceReceivedEvent;
    protected Product _lastProduct;
    protected ResourceReceivedEvent _lastSetupResourceReceivedEvent;
    protected Infinite _off;
    protected ProductRelatedProcess _processing;
    private IDictionary<IProductType, IRealDistribution> _processingDictionary;
    protected ResourceManager _processingResourceManager;
    private IDictionary<IResourcePool, IDictionary<IResourceType, int>> _processingResourcesDictionary;
    private IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> _productTypeSpecificProcessingResourcesDictionary;
    protected ProductRelatedProcess _setup;
    protected ResourceManager _setupResourceManager;
    private IDictionary<IResourcePool, IDictionary<IResourceType, int>> _setupResourcesDictionary;
    private IDictionary<IProductType, IRealDistribution> _setupsDictionary;

    public IEnumerable<Resource> AvailableProcessingResources
    {
      get { return _processingResourceManager.AvailableResources; }
    }

    public override void Initialize()
    {
      base.Initialize();
      _arrivedProducts = new Queue<Product>();
      _blockedProducts = new Queue<Product>();

      _blocked = new Infinite("Blocked", this, () => new BlockedStartEvent(this), startEvent => new BlockedEndEvent(this, startEvent));
      _idle = new Infinite("Idle", this, () => new IdleStartEvent(this), startEvent => new IdleEndEvent(this, startEvent));
      _processing = new ProductRelatedProcess("Processing", this, () => new ProcessingStartEvent(this, _arrivedProducts.ToArray()), startEvent => new ProcessingEndEvent(this, _arrivedProducts.ToArray(), startEvent), () => new ProductsDestroyedEvent(this, _arrivedProducts.ToArray()))
                    {
                      DurationDistribution = ProcessingDurationDistribution,
                      ProductTypeDistributions = ProcessingsDictionary
                    };

      _awaitingProcessingResources = new Infinite("Awaiting Processing Resources", this, () => new ProcessingResourceRequestedEvent(_lastReceivedProduct.ProductType, this, _processingResourceManager.RequestedResources), startEvent =>
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                              _lastProcessingResourceReceivedEvent = new ProcessingResourceReceivedEvent(_lastReceivedProduct.ProductType, this, startEvent, _processingResourceManager.AvailableResources, _processingResourceManager.RequestedResources);
                                                                                                                                                                                                                              return _lastProcessingResourceReceivedEvent;
                                                                                                                                                                                                                            });
      _awaitingSetupResources = new Infinite("Awaiting Setup Resources", this, () => new SetupResourceRequestedEvent(_lastReceivedProduct.ProductType, this, _setupResourceManager.RequestedResources), startEvent =>
                                                                                                                                                                                                        {
                                                                                                                                                                                                          _lastSetupResourceReceivedEvent = new SetupResourceReceivedEvent(_lastReceivedProduct.ProductType, this, startEvent, _setupResourceManager.AvailableResources, _setupResourceManager.RequestedResources);
                                                                                                                                                                                                          return _lastSetupResourceReceivedEvent;
                                                                                                                                                                                                        });
      _processingResourceManager = new ResourceManager(_awaitingProcessingResources, _processing.Start)
                                   {
                                     RequestedResources = ProcessingResourcesDictionary
                                   };

      _off = new Infinite("Off", this, () => new OffStartEvent(this), startEvent => new OffEndEvent(this, startEvent));

      _idle.OnEnter = RaiseGotAvailable;

      if (HasSetup)
      {
        _setup = new ProductRelatedProcess("Setup", this, () => new SetupStartEvent(this, _arrivedProducts.ToArray()), arg => new SetupEndEvent(this, arg, _arrivedProducts.ToArray()), () => new SetupCancelEvent(this));
        _setupResourceManager = new ResourceManager(_awaitingSetupResources, _setup.Start)
                                {
                                  RequestedResources = SetupResourcesDictionary
                                };
        PrepareSetup();
      }

      if (CanFail)
      {
        _failure = new RecurringTemporal("Failure", this, FailureOccurrenceDistribution, FailureDurationDistribution, () => new FailureStartEvent(this), startEvent => new FailureEndEvent(this, startEvent));
      }
    }

    public override void Receive(Product product)
    {
      base.Receive(product);
      if (_arrivedProducts.Contains(product))
      {
        throw new InvalidOperationException("This should not occur");
      }
    }

    public void OnWorkingTimeEnded()
    {
      if (!IsWorkingTimeDependent)
      {
        return;
      }
      ShutDown();
    }

    public void OnWorkingTimeStarted()
    {
      if (!IsWorkingTimeDependent)
      {
        return;
      }

      StartUp();
    }

    public override void Reset()
    {
      FailureOccurrenceDistribution = null;
      FailureDurationDistribution = null;
      ProcessingDurationDistribution = null;
      BatchSizeDistribution = null;
      SetupDurationDistribution = null;
      _currentBatchSize = null;
      _arrivedProducts = null;
      _off = null;
      _idle = null;
      _failure = null;
      _setup = null;
      _blocked = null;
      _blockedProducts = null;
      _lastProduct = null;
      _processing = null;
      _batchesDictionary = null;
      _processingDictionary = null;
      _setupsDictionary = null;
      _processingResourceManager = null;
      _awaitingProcessingResources = null;
      _awaitingSetupResources = null;
      base.Reset();
    }

    [JsonProperty]
    public IDistributionConfiguration ProcessingDuration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration SetupDuration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration FailureOccurrence
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration FailureDuration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool CanFail
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool HasSetup
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool IsWorkingTimeDependent
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool IsDemandingProcessingResourcesInSetup
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration BatchSize
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    public IEnumerable<IInfluenceRate> Influences
    {
      get { return _influences; }
    }

    public IEnumerable<IProductTypeDistribution> ProcessingDurations
    {
      get { return _ProcessingDurations; }
    }

    public IDictionary<IProductType, IRealDistribution> ProcessingsDictionary
    {
      get { return _processingDictionary ?? (_processingDictionary = new Dictionary<IProductType, IRealDistribution>()); }
    }

    [JsonProperty]
    public INamedProcessConfiguration Maintenance
    {
      get { return Get<INamedProcessConfiguration>(); }
      set { Set(value); }
    }

    public void AddProcessing(IProductTypeDistribution productTypeDistribution)
    {
      if (_ProcessingDurations.Any(cm => cm.ProductType == productTypeDistribution.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _ProcessingDurations.Add(productTypeDistribution);
    }

    public void RemoveProcessing(IProductTypeDistribution productTypeDistribution)
    {
      if (!_ProcessingDurations.Contains(productTypeDistribution))
      {
        throw new InvalidOperationException("The given productTypeDistribution does not exist.");
      }
      _ProcessingDurations.Remove(productTypeDistribution);
    }

    public IEnumerable<IProductTypeDistribution> SetupDurations
    {
      get { return _SetupDurations; }
    }

    public IDictionary<IProductType, IRealDistribution> SetupsDictionary
    {
      get { return _setupsDictionary ?? (_setupsDictionary = new Dictionary<IProductType, IRealDistribution>()); }
    }

    public IDictionary<IResourcePool, IDictionary<IResourceType, int>> SetupResourcesDictionary
    {
      get { return _setupResourcesDictionary ?? (_setupResourcesDictionary = new Dictionary<IResourcePool, IDictionary<IResourceType, int>>()); }
    }

    public IDictionary<IResourcePool, IDictionary<IResourceType, int>> ProcessingResourcesDictionary
    {
      get { return _processingResourcesDictionary ?? (_processingResourcesDictionary = new Dictionary<IResourcePool, IDictionary<IResourceType, int>>()); }
    }

    public IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> ProductTypeSpecificProcessingResourcesDictionary
    {
      get { return _productTypeSpecificProcessingResourcesDictionary ?? (_productTypeSpecificProcessingResourcesDictionary = new Dictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, int>>>()); }
    }

    public void AddSetup(IProductTypeDistribution productTypeDistribution)
    {
      if (_SetupDurations.Any(cm => cm.ProductType == productTypeDistribution.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _SetupDurations.Add(productTypeDistribution);
    }

    public void RemoveSetup(IProductTypeDistribution productTypeDistribution)
    {
      if (!_SetupDurations.Contains(productTypeDistribution))
      {
        throw new InvalidOperationException("The given productTypeDistribution does not exist.");
      }
      _SetupDurations.Remove(productTypeDistribution);
    }

    public IEnumerable<IProductTypeDistribution> BatchSizes
    {
      get { return _BatchSizes; }
    }

    public IDictionary<IProductType, IRealDistribution> BatchesDictionary
    {
      get { return _batchesDictionary ?? (_batchesDictionary = new Dictionary<IProductType, IRealDistribution>()); }
    }

    public void AddInfluence(IInfluenceRate influence)
    {
      _influences.Add(influence);
    }

    public void RemoveInfluence(IInfluenceRate influence)
    {
      _influences.Remove(influence);
    }

    public void AddBatchSize(IProductTypeDistribution productTypeDistribution)
    {
      if (_BatchSizes.Any(cm => cm.ProductType == productTypeDistribution.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _BatchSizes.Add(productTypeDistribution);
    }

    public void RemoveBatchSize(IProductTypeDistribution productTypeDistribution)
    {
      if (!_BatchSizes.Contains(productTypeDistribution))
      {
        throw new InvalidOperationException("The given productTypeDistribution does not exist.");
      }
      _BatchSizes.Remove(productTypeDistribution);
    }

    public IEnumerable<IResourcePoolResourceTypeAmount> ProcessingResources
    {
      get { return _processingResources; }
    }

    public void AddProcessingResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      if (_processingResources.Any(cm => cm.ResourceType == resourceResourceTypeAmount.ResourceType && cm.ResourcePool == resourceResourceTypeAmount.ResourcePool))
      {
        throw new InvalidOperationException("An identical resource type already exists!");
      }
      _processingResources.Add(resourceResourceTypeAmount);
      ResourceAdded.Raise(this, resourceResourceTypeAmount.ResourcePool);
    }

    public void AddProcessingResource(IProductTypeSpecificResource resource)
    {
      if (_productTypeSpecificProcessingResources.Any(r => r.ProductType == resource.ProductType && r.ResourcePool == resource.ResourcePool && r.ResourceType == resource.ResourceType))
      {
        throw new InvalidOperationException("An identical resource already exists!");
      }

      _productTypeSpecificProcessingResources.Add(resource);

      ResourceAdded.Raise(this, resource.ResourcePool);
    }

    public event Action<IWorkstationBase, IResourcePool> ResourceAdded;

    public event Action<IWorkstationBase, IResourcePool> ResourceRemoved;

    public IEnumerable<IProductTypeSpecificResource> ProductTypeSpecificProcessingResources
    {
      get { return _productTypeSpecificProcessingResources; }
    }

    public void RemoveProcessingResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      if (!_processingResources.Contains(resourceResourceTypeAmount))
      {
        throw new InvalidOperationException("The given resourceType does not exist.");
      }
      _processingResources.Remove(resourceResourceTypeAmount);
      ResourceRemoved.Raise(this, resourceResourceTypeAmount.ResourcePool);
    }

    public void RemoveProcessingResource(IProductTypeSpecificResource resource)
    {
      if (!_productTypeSpecificProcessingResources.Contains(resource))
      {
        throw new InvalidOperationException("The given resourceType does not exist.");
      }
      _productTypeSpecificProcessingResources.Remove(resource);

      ResourceRemoved.Raise(this, resource.ResourcePool);
    }

    public IEnumerable<IResourcePoolResourceTypeAmount> SetupResources
    {
      get { return _SetupResources; }
    }

    public void AddSetupResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      if (_SetupResources.Any(cm => cm.ResourceType == resourceResourceTypeAmount.ResourceType))
      {
        throw new InvalidOperationException("An identical resource type already exists!");
      }
      _SetupResources.Add(resourceResourceTypeAmount);
    }

    public void RemoveSetupResource(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      if (!_SetupResources.Contains(resourceResourceTypeAmount))
      {
        throw new InvalidOperationException("The given resourceType does not exist.");
      }
      _SetupResources.Remove(resourceResourceTypeAmount);
    }

    public IRealDistribution BatchSizeDistribution { get; set; }
    public IRealDistribution FailureDurationDistribution { get; set; }
    public IRealDistribution FailureOccurrenceDistribution { get; set; }
    public IRealDistribution ProcessingDurationDistribution { get; set; }
    public IRealDistribution SetupDurationDistribution { get; set; }

    protected double GetBatchSize(Product product)
    {
      if (!_currentBatchSize.HasValue)
      {
        _currentBatchSize = _batchesDictionary.ContainsKey(product.ProductType)
                              ? _batchesDictionary[product.ProductType].GetSample()
                              : BatchSizeDistribution.GetSample();
        _currentBatchSize = Math.Round(_currentBatchSize.Value);
      }
      return _currentBatchSize.Value;
    }

    protected override IEnumerable<Product> GetResidingProducts()
    {
      if (_arrivedProducts.Intersect(_blockedProducts)
                          .Any())
      {
        throw new InvalidOperationException("This should not occur. A product can not be in ArrivedProducts and BlockedProducts simultaneously.");
      }

      return _arrivedProducts.Concat(_blockedProducts);
    }

    protected virtual bool NeedSetup(Product product)
    {
      return HasSetup && (_lastProduct == null || product.ProductType != _lastProduct.ProductType);
    }

    //todo: move this to errorvalidation or factory
    protected void CheckBatchSizes()
    {
      if (!BatchesDictionary.Any() &&
          BatchSizeDistribution == null)
      {
        throw new ModelConfigurationException(Model, this, $"Batch sizes in workstation {Name} are not well defined. Please add a default distribution or add at least one product type specific one.", "BatchSizes");
      }
    }

    protected virtual void PrepareSetup()
    {
      if (SetupDurationDistribution != null)
      {
        _setup.DurationDistribution = SetupDurationDistribution;
      }
      _setup.ProductTypeDistributions = SetupsDictionary;
    }
    
    protected void SetProcessingResources(IProductType productType)
    {
      _processingResourceManager.RequestedResources = ProductTypeSpecificProcessingResourcesDictionary.ContainsKey(productType)
                                                        ? ProductTypeSpecificProcessingResourcesDictionary[productType]
                                                        : ProcessingResourcesDictionary;
    }

    protected abstract void ShutDown();

    protected abstract void StartUp();
  }
}