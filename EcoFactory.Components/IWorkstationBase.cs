#region License

// Copyright (c) 2013 HTW Berlin All rights reserved.

#endregion License

using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Resources;
using Milan.Simulation.ShiftSystems;
using System;
using System.Collections.Generic;

namespace EcoFactory.Components
{
  public interface IWorkstationBase : IStationaryElement, IWorkingTimeDependent, IInfluenceAware
  {
    IDistributionConfiguration ProcessingDuration { get; set; }
    IDistributionConfiguration BatchSize { get; set; }
    IDistributionConfiguration SetupDuration { get; set; }
    IDistributionConfiguration FailureOccurrence { get; set; }
    IDistributionConfiguration FailureDuration { get; set; }
    bool CanFail { get; set; }
    bool HasSetup { get; set; }
    bool IsDemandingProcessingResourcesInSetup { get; set; }
    IRealDistribution BatchSizeDistribution { get; set; }
    IRealDistribution FailureDurationDistribution { get; set; }
    IRealDistribution FailureOccurrenceDistribution { get; set; }
    IRealDistribution ProcessingDurationDistribution { get; set; }
    IRealDistribution SetupDurationDistribution { get; set; }
    IEnumerable<IProductTypeDistribution> ProcessingDurations { get; }
    IDictionary<IProductType, IRealDistribution> ProcessingsDictionary { get; }
    IEnumerable<IProductTypeDistribution> SetupDurations { get; }
    IDictionary<IProductType, IRealDistribution> SetupsDictionary { get; }
    IEnumerable<IProductTypeDistribution> BatchSizes { get; }
    IDictionary<IProductType, IRealDistribution> BatchesDictionary { get; }
    IEnumerable<IResourcePoolResourceTypeAmount> ProcessingResources { get; }
    IEnumerable<IResourcePoolResourceTypeAmount> SetupResources { get; }
    INamedProcessConfiguration Maintenance { get; set; }
    IDictionary<IProductType, IDictionary<IResourcePool, IDictionary<IResourceType, int>>> ProductTypeSpecificProcessingResourcesDictionary { get; }
    IDictionary<IResourcePool, IDictionary<IResourceType, int>> ProcessingResourcesDictionary { get; }
    IDictionary<IResourcePool, IDictionary<IResourceType, int>> SetupResourcesDictionary { get; }
    
    void AddProcessing(IProductTypeDistribution productTypeDistribution);

    void RemoveProcessing(IProductTypeDistribution productTypeDistribution);

    void AddSetup(IProductTypeDistribution productTypeDistribution);

    void RemoveSetup(IProductTypeDistribution productTypeDistribution);

    void AddBatchSize(IProductTypeDistribution productTypeDistribution);

    void RemoveBatchSize(IProductTypeDistribution productTypeDistribution);

    void AddProcessingResource(IResourcePoolResourceTypeAmount resource);

    void AddProcessingResource(IProductTypeSpecificResource resource);

    void RemoveProcessingResource(IResourcePoolResourceTypeAmount resource);

    void RemoveProcessingResource(IProductTypeSpecificResource resource);

    void AddSetupResource(IResourcePoolResourceTypeAmount resource);

    void RemoveSetupResource(IResourcePoolResourceTypeAmount resource);

    event Action<IWorkstationBase, IResourcePool> ResourceAdded;

    event Action<IWorkstationBase, IResourcePool> ResourceRemoved;

    IEnumerable<IProductTypeSpecificResource> ProductTypeSpecificProcessingResources { get; }
  }
}