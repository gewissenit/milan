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

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class EntryPointFactory : EntityFactory
  {
    private readonly IConnectionFactory _connectionFactory;
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;

    [ImportingConstructor]
    public EntryPointFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions,
                             [ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories,
                             [Import] IProductTypeDistributionFactory productTypeDistributionFactory,
                             [Import] IConnectionFactory connectionFactory)
      : base("Entry Point", additionalEntityDuplicationActions)
    {
      _distributionFactories = distributionFactories;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _connectionFactory = connectionFactory;
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is EntryPoint;
    }

    private EntryPoint Clone(IEntryPoint master)
    {
      var clone = new EntryPoint
                  {
                    IsWorkingTimeDependent = master.IsWorkingTimeDependent
                  };

      if (master.BatchSize != null)
      {
        clone.BatchSize = _distributionFactories.Single(df => df.CanHandle(master.BatchSize))
                                                 .DuplicateConfiguration(master.BatchSize);
      }

      foreach (var productTypeDistribution in master.ArrivalOccurrences)
      {
        clone.AddArrival(_productTypeDistributionFactory.Duplicate(productTypeDistribution));
      }

      foreach (var batchSize in master.BatchSizes)
      {
        clone.AddBatchSize(_productTypeDistributionFactory.Duplicate(batchSize));
      }

      Milan.Simulation.Utils.CloneStationaryElement(master, clone, _connectionFactory);

      return clone;
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone((IEntryPoint) entity);
    }

    public override void ResolveReferences(IEntity entity)
    {
      var ep = (IEntryPoint) entity;
      Milan.Simulation.Utils.PrepareStationaryElement(ep);
      foreach (var productTypeDistribution in ep.ArrivalOccurrences.Concat(ep.BatchSizes))
      {
        var productType = ep.Model.Entities.OfType<IProductType>()
                            .Single(e => e.Id == productTypeDistribution.ProductType.Id);
        productTypeDistribution.ProductType = productType;
      }
    }

    public override void Prepare(IEntity entity)
    {
      ResolveReferences(entity);
      var ep = (IEntryPoint) entity;

      var id = ep.Id.ToString();
      ep.BatchSizes.ForEach(
                            productTypeDistribution =>
                            ep.BatchesDictionary.Add(productTypeDistribution.ProductType,
                                                     Milan.Simulation.Utils.CreateAndSetupDistribution(
                                                                                                              productTypeDistribution
                                                                                                                .DistributionConfiguration,
                                                                                                              _distributionFactories,
                                                                                                              ep)));
      ep.ArrivalOccurrences.ForEach(
                                    productTypeDistribution =>
                                    ep.ArrivalsDictionary.Add(productTypeDistribution.ProductType,
                                                              Milan.Simulation.Utils.CreateAndSetupDistribution(
                                                                                                                       productTypeDistribution
                                                                                                                         .DistributionConfiguration,
                                                                                                                       _distributionFactories,
                                                                                                                       ep)));

      ep.BatchSizeDistribution = Milan.Simulation.Utils.CreateAndSetupDistribution(ep.BatchSize, _distributionFactories, ep);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      var master = (IEntryPoint) entity;
      var clone = Clone(master);
      return clone;
    }

    protected override IEntity Create()
    {
      return new EntryPoint();
    }
  }
}