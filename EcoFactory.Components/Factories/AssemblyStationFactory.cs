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
  internal class AssemblyStationFactory : EntityFactory
  {
    private readonly IConnectionFactory _connectionFactory;
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;
    private readonly IEnumerable<IDistributionFactory<IIntDistribution>> _intDistributionFactories;
    private readonly ITransformationRuleFactory _transformationRuleFactory;

    [ImportingConstructor]
    public AssemblyStationFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions,
                                  [ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories,
                                  [ImportMany] IEnumerable<IDistributionFactory<IIntDistribution>> intDistributionFactories,
                                  [Import] ITransformationRuleFactory transformationRuleFactory,
                                  [Import] IConnectionFactory connectionFactory)
      : base("Assembly Station", additionalEntityDuplicationActions)
    {
      _distributionFactories = distributionFactories;
      _intDistributionFactories = intDistributionFactories;
      _transformationRuleFactory = transformationRuleFactory;
      _connectionFactory = connectionFactory;
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is AssemblyStation;
    }

    private AssemblyStation Clone(IAssemblyStation master)
    {
      var clone = new AssemblyStation
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
      var ass = (IAssemblyStation) entity;
      var productTypes = ass.Model.Entities.OfType<IProductType>()
                            .ToArray();
      foreach (var transformationRule in ass.TransformationRules)
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
            var resourcePool = ass.Model.Entities.OfType<IResourcePool>()
                                              .Single(e => e.Id == resource.ResourcePool.Id);
            var resourceType = ass.Model.Entities.OfType<IResourceType>()
                                              .Single(e => e.Id == resource.ResourceType.Id);

            if (!ass.TransformationRuleSpecificProcessingResourcesDictionary.ContainsKey(output))
            {
              ass.TransformationRuleSpecificProcessingResourcesDictionary.Add(output, new Dictionary<IResourcePool, IDictionary<IResourceType, int>>());
            }
            if (!ass.TransformationRuleSpecificProcessingResourcesDictionary[output].ContainsKey(resourcePool))
            {
              ass.TransformationRuleSpecificProcessingResourcesDictionary[output].Add(resourcePool, new Dictionary<IResourceType, int>());
            }
            ass.TransformationRuleSpecificProcessingResourcesDictionary[output][resourcePool].Add(resourceType, resource.Amount);
          }
        }
      }

      Utils.PrepareWorkstationBase(ass);
    }

    public override void Prepare(IEntity entity)
    {
      ResolveReferences(entity);
      var ass = (IAssemblyStation) entity;
      
      Utils.PrepareWorkstationBaseDistributions(ass, _distributionFactories);

      var id = ass.Id.ToString();
      foreach (var transformationRule in ass.TransformationRules)
      {
        foreach (var output in transformationRule.Outputs)
        {
          output.Distribution = _distributionFactories.Single(df => df.CanHandle(output.ProcessingDuration))
                                                       .CreateAndConfigureDistribution(output.ProcessingDuration);
          output.Distribution.Seed = ass.CurrentExperiment.AcquireInitializationSeed(id);
        }

        transformationRule.Distribution = (IEmpiricalIntDistribution) _intDistributionFactories.First()
                                                                                                .CreateDistribution();
        transformationRule.Distribution.Seed = ass.CurrentExperiment.AcquireInitializationSeed(id);
      }
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone((IAssemblyStation) entity);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      var master = (IAssemblyStation) entity;
      var clone = Clone(master);
      return clone;
    }

    protected override IEntity Create()
    {
      return new AssemblyStation();
    }
  }
}