using System.Collections.Generic;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Factories;

namespace EcoFactory.Components
{
  internal static class Utils
  {
    internal static void PrepareWorkstationBase(IWorkstationBase workstationBase)
    {
      foreach (var productTypeDistribution in workstationBase.SetupDurations.Concat(workstationBase.BatchSizes)
                                                             .Concat(workstationBase.ProcessingDurations))
      {
        var productType = workstationBase.Model.Entities.OfType<IProductType>()
                                         .Single(e => e.Id == productTypeDistribution.ProductType.Id);
        productTypeDistribution.ProductType = productType;
      }
      
      foreach (var resource in workstationBase.ProductTypeSpecificProcessingResources)
      {
        var productType = workstationBase.Model.Entities.OfType<IProductType>()
                                         .Single(e => e.Id == resource.ProductType.Id);
        var resourcePool = workstationBase.Model.Entities.OfType<IResourcePool>()
                                          .Single(e => e.Id == resource.ResourcePool.Id);
        var resourceType = workstationBase.Model.Entities.OfType<IResourceType>()
                                          .Single(e => e.Id == resource.ResourceType.Id);

        if (!workstationBase.ProductTypeSpecificProcessingResourcesDictionary.ContainsKey(productType))
        {
          workstationBase.ProductTypeSpecificProcessingResourcesDictionary.Add(productType, new Dictionary<IResourcePool, IDictionary<IResourceType, int>>());
        }
        if (!workstationBase.ProductTypeSpecificProcessingResourcesDictionary[productType].ContainsKey(resourcePool))
        {
          workstationBase.ProductTypeSpecificProcessingResourcesDictionary[productType].Add(resourcePool, new Dictionary<IResourceType, int>());
        }
        workstationBase.ProductTypeSpecificProcessingResourcesDictionary[productType][resourcePool].Add(resourceType, resource.Amount);
      }
      
      foreach (var resource in workstationBase.ProcessingResources)
      {
        var resourcePool = workstationBase.Model.Entities.OfType<IResourcePool>()
                                          .Single(e => e.Id == resource.ResourcePool.Id);
        var resourceType = workstationBase.Model.Entities.OfType<IResourceType>()
                                          .Single(e => e.Id == resource.ResourceType.Id);
        
        if (!workstationBase.ProcessingResourcesDictionary.ContainsKey(resourcePool))
        {
          workstationBase.ProcessingResourcesDictionary.Add(resourcePool, new Dictionary<IResourceType, int>());
        }
        workstationBase.ProcessingResourcesDictionary[resourcePool].Add(resourceType, resource.Amount);
      }
      
      foreach (var resource in workstationBase.SetupResources)
      {
        var resourcePool = workstationBase.Model.Entities.OfType<IResourcePool>()
                                          .Single(e => e.Id == resource.ResourcePool.Id);
        var resourceType = workstationBase.Model.Entities.OfType<IResourceType>()
                                          .Single(e => e.Id == resource.ResourceType.Id);
        
        if (!workstationBase.SetupResourcesDictionary.ContainsKey(resourcePool))
        {
          workstationBase.SetupResourcesDictionary.Add(resourcePool, new Dictionary<IResourceType, int>());
        }
        workstationBase.SetupResourcesDictionary[resourcePool].Add(resourceType, resource.Amount);
      }
      
      Milan.Simulation.Utils.PrepareStationaryElement(workstationBase);
    }

    internal static void PrepareWorkstationBaseDistributions(IWorkstationBase workstationBase, IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories)
    {
      if (workstationBase.BatchSize != null)
      {
        workstationBase.BatchSizeDistribution = Milan.Simulation.Utils.CreateAndSetupDistribution(workstationBase.BatchSize, distributionFactories, workstationBase);
      }
      if (workstationBase.CanFail)
      {
        workstationBase.FailureDurationDistribution = Milan.Simulation.Utils.CreateAndSetupDistribution(workstationBase.FailureDuration, distributionFactories, workstationBase);
        workstationBase.FailureOccurrenceDistribution = Milan.Simulation.Utils.CreateAndSetupDistribution(workstationBase.FailureOccurrence, distributionFactories, workstationBase);
      }
      if (workstationBase.HasSetup)
      {
        workstationBase.SetupDurationDistribution = Milan.Simulation.Utils.CreateAndSetupDistribution(workstationBase.SetupDuration, distributionFactories, workstationBase);
      }

      if (workstationBase.ProcessingDuration != null)
      {
        workstationBase.ProcessingDurationDistribution = Milan.Simulation.Utils.CreateAndSetupDistribution(workstationBase.ProcessingDuration, distributionFactories, workstationBase);
      }

      workstationBase.ProcessingDurations.ForEach(ptd => workstationBase.ProcessingsDictionary.Add(ptd.ProductType, Milan.Simulation.Utils.CreateAndSetupDistribution(ptd.DistributionConfiguration, distributionFactories, workstationBase)));
      workstationBase.BatchSizes.ForEach(ptd => workstationBase.BatchesDictionary.Add(ptd.ProductType, Milan.Simulation.Utils.CreateAndSetupDistribution(ptd.DistributionConfiguration, distributionFactories, workstationBase)));
      workstationBase.SetupDurations.ForEach(ptd => workstationBase.SetupsDictionary.Add(ptd.ProductType, Milan.Simulation.Utils.CreateAndSetupDistribution(ptd.DistributionConfiguration, distributionFactories, workstationBase)));
    }

    internal static void CloneWorkstationBase(IWorkstationBase master, IWorkstationBase clone, IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories, IProductTypeDistributionFactory productTypeDistributionFactory, IResourcePoolResourceTypeAmountFactory resourcePoolResourceTypeAmountFactory, IProductTypeSpecificResourceFactory productTypeSpecificResourceTypeAmountFactory)
    {
      //todo: simulation entities do not need dist clones
      clone.IsWorkingTimeDependent = master.IsWorkingTimeDependent;
      clone.CanFail = master.CanFail;
      clone.HasSetup = master.HasSetup;
      clone.IsDemandingProcessingResourcesInSetup = master.IsDemandingProcessingResourcesInSetup;

      if (master.FailureDuration != null)
      {
        clone.FailureDuration = distributionFactories.Single(df => df.CanHandle(master.FailureDuration))
                                                     .DuplicateConfiguration(master.FailureDuration);
      }

      if (master.FailureOccurrence != null)
      {
        clone.FailureOccurrence = distributionFactories.Single(df => df.CanHandle(master.FailureOccurrence))
                                                       .DuplicateConfiguration(master.FailureOccurrence);
      }

      if (master.SetupDuration != null)
      {
        clone.SetupDuration = distributionFactories.Single(df => df.CanHandle(master.SetupDuration))
                                                   .DuplicateConfiguration(master.SetupDuration);
      }

      if (master.ProcessingDuration != null)
      {
        clone.ProcessingDuration = distributionFactories.Single(df => df.CanHandle(master.ProcessingDuration))
                                                        .DuplicateConfiguration(master.ProcessingDuration);
      }

      if (master.BatchSize != null)
      {
        clone.BatchSize = distributionFactories.Single(df => df.CanHandle(master.BatchSize))
                                               .DuplicateConfiguration(master.BatchSize);
      }

      foreach (var influence in master.Influences)
      {
        clone.AddInfluence(influence);
      }

      foreach (var productTypeDistribution in master.ProcessingDurations)
      {
        clone.AddProcessing(productTypeDistributionFactory.Duplicate(productTypeDistribution));
      }

      foreach (var batchSize in master.BatchSizes)
      {
        clone.AddBatchSize(productTypeDistributionFactory.Duplicate(batchSize));
      }

      foreach (var productTypeDistribution in master.SetupDurations)
      {
        clone.AddSetup(productTypeDistributionFactory.Duplicate(productTypeDistribution));
      }

      foreach (var resourcePoolResourceTypeAmount in master.ProcessingResources)
      {
        clone.AddProcessingResource(resourcePoolResourceTypeAmountFactory.Duplicate(resourcePoolResourceTypeAmount));
      }

      foreach (var productTypeSpecificProcessingResourceTypeAmount in master.ProductTypeSpecificProcessingResources)
      {
        clone.AddProcessingResource(productTypeSpecificResourceTypeAmountFactory.Duplicate(productTypeSpecificProcessingResourceTypeAmount));
      }

      foreach (var resourcePoolResourceTypeAmount in master.SetupResources)
      {
        clone.AddSetupResource(resourcePoolResourceTypeAmountFactory.Duplicate(resourcePoolResourceTypeAmount));
      }
    }
  }
}