#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class ProbabilityAssemblyStationFactory : EntityFactory
  {
    private readonly IConnectionFactory _connectionFactory;
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;
    private readonly IEnumerable<IDistributionFactory<IIntDistribution>> _intDistributionFactories;
    private readonly ITransformationRuleFactory _transformationRuleFactory;

    [ImportingConstructor]
    public ProbabilityAssemblyStationFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions,
                                             [ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories,
                                             [ImportMany] IEnumerable<IDistributionFactory<IIntDistribution>> intDistributionFactories,
                                             [Import] ITransformationRuleFactory transformationRuleFactory,
                                             [Import] IConnectionFactory connectionFactory)
      : base("Probability Assembly Station", additionalEntityDuplicationActions)
    {
      _distributionFactories = distributionFactories;
      _intDistributionFactories = intDistributionFactories;
      _transformationRuleFactory = transformationRuleFactory;
      _connectionFactory = connectionFactory;
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is ProbabilityAssemblyStation;
    }

    private ProbabilityAssemblyStation Clone(IProbabilityAssemblyStation master)
    {
      var clone = new ProbabilityAssemblyStation
                  {
                    IsWorkingTimeDependent = master.IsWorkingTimeDependent,
                    CanFail = master.CanFail,
                    HasSetup = master.HasSetup
                  };

      if (master.FailureDuration != null)
      {
        clone.FailureDuration = _distributionFactories.Single(df => df.CanHandle(master.FailureDuration))
                                                       .DuplicateConfiguration(master.FailureDuration);
      }

      if (master.FailureOccurrence != null)
      {
        clone.FailureOccurrence = _distributionFactories.Single(df => df.CanHandle(master.FailureOccurrence))
                                                         .DuplicateConfiguration(master.FailureOccurrence);
      }

      if (master.SetupDuration != null)
      {
        clone.SetupDuration = _distributionFactories.Single(df => df.CanHandle(master.SetupDuration))
                                                     .DuplicateConfiguration(master.SetupDuration);
      }

      foreach (var transformationRule in master.TransformationRules)
      {
        clone.AddTransformationRule(_transformationRuleFactory.Duplicate(transformationRule));
      }
      
      Milan.Simulation.Utils.CloneStationaryElement(master, clone, _connectionFactory);

      return clone;
    }

    public override void ResolveReferences(IEntity entity)
    {
      var pas = (IProbabilityAssemblyStation) entity;
      var productTypes = pas.Model.Entities.OfType<IProductType>()
                            .ToArray();
      foreach (var transformationRule in pas.TransformationRules)
      {
        foreach (var productTypeAmount in transformationRule.Inputs)
        {
          var productType = productTypes.Single(pt => pt.Id == productTypeAmount.ProductType.Id);
          productTypeAmount.ProductType = productType;
        }

        foreach (var output in transformationRule.Outputs)
        {
          foreach (var productTypeAmount in output.Outputs)
          {
            var productType = productTypes.Single(pt => pt.Id == productTypeAmount.ProductType.Id);
            productTypeAmount.ProductType = productType;
          }

          foreach (var resource in output.Resources)
          {
            var resourcePool = pas.Model.Entities.OfType<IResourcePool>()
                                              .Single(e => e.Id == resource.ResourcePool.Id);
            var resourceType = pas.Model.Entities.OfType<IResourceType>()
                                              .Single(e => e.Id == resource.ResourceType.Id);

            if (!pas.TransformationRuleSpecificProcessingResourcesDictionary.ContainsKey(output))
            {
              pas.TransformationRuleSpecificProcessingResourcesDictionary.Add(output, new Dictionary<IResourcePool, IDictionary<IResourceType, int>>());
            }
            if (!pas.TransformationRuleSpecificProcessingResourcesDictionary[output].ContainsKey(resourcePool))
            {
              pas.TransformationRuleSpecificProcessingResourcesDictionary[output].Add(resourcePool, new Dictionary<IResourceType, int>());
            }
            pas.TransformationRuleSpecificProcessingResourcesDictionary[output][resourcePool].Add(resourceType, resource.Amount);
          }
        }
      }

      Utils.PrepareWorkstationBase(pas);
    }

    public override void Prepare(IEntity entity)
    {
      ResolveReferences(entity);
      var pas = (IProbabilityAssemblyStation) entity;
      
      Utils.PrepareWorkstationBaseDistributions(pas, _distributionFactories);

      var id = pas.Id.ToString();
      foreach (var transformationRule in pas.TransformationRules)
      {
        foreach (var output in transformationRule.Outputs)
        {
          output.Distribution = _distributionFactories.Single(df => df.CanHandle(output.ProcessingDuration))
                                                       .CreateAndConfigureDistribution(output.ProcessingDuration);
          output.Distribution.Seed = pas.CurrentExperiment.AcquireInitializationSeed(id);
        }

        transformationRule.Distribution = (IEmpiricalIntDistribution) _intDistributionFactories.First()
                                                                                                .CreateDistribution();
        transformationRule.Distribution.Seed = pas.CurrentExperiment.AcquireInitializationSeed(id);
      }

      pas.TransformationSelectionDistribution = (IEmpiricalIntDistribution) _intDistributionFactories.First()
                                                                                                      .CreateDistribution();
      pas.TransformationSelectionDistribution.Seed = pas.CurrentExperiment.AcquireInitializationSeed(id);
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone((IProbabilityAssemblyStation) entity);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      var master = (IProbabilityAssemblyStation) entity;
      var clone = Clone(master);
      return clone;
    }

    protected override IEntity Create()
    {
      return new ProbabilityAssemblyStation();
    }
  }
}