#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources.Factories;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class InhomogeneousWorkstationFactory : EntityFactory
  {
    private readonly IConnectionFactory _connectionFactory;
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;
    private readonly IProductTypeDistributionFactory _productTypeDistributionFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourcePoolResourceTypeAmountFactory;
    private readonly IProductTypeSpecificResourceFactory _productTypeSpecificResourceTypeAmountFactory;

    [ImportingConstructor]
    public InhomogeneousWorkstationFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions, [ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories, [Import] IProductTypeDistributionFactory productTypeDistributionFactory, [Import] IConnectionFactory connectionFactory, [Import] IResourcePoolResourceTypeAmountFactory resourcePoolResourceTypeAmountFactory, [Import] IProductTypeSpecificResourceFactory productTypeSpecificResourceTypeAmountFactory)
      : base("Inhomogeneous Workstation", additionalEntityDuplicationActions)
    {
      _distributionFactories = distributionFactories;
      _productTypeDistributionFactory = productTypeDistributionFactory;
      _connectionFactory = connectionFactory;
      _resourcePoolResourceTypeAmountFactory = resourcePoolResourceTypeAmountFactory;
      _productTypeSpecificResourceTypeAmountFactory = productTypeSpecificResourceTypeAmountFactory;
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is InhomogeneousWorkstation;
    }

    private InhomogeneousWorkstation Clone(IInhomogeneousWorkstation master)
    {
      var clone = new InhomogeneousWorkstation();
      Utils.CloneWorkstationBase(master, clone, _distributionFactories, _productTypeDistributionFactory, _resourcePoolResourceTypeAmountFactory, _productTypeSpecificResourceTypeAmountFactory);
      Milan.Simulation.Utils.CloneStationaryElement(master, clone, _connectionFactory);
      return clone;
    }

    public override void ResolveReferences(IEntity entity)
    {
      var workstationBase = (IWorkstationBase) entity;
      Utils.PrepareWorkstationBase(workstationBase);
    }

    public override void Prepare(IEntity entity)
    {
      ResolveReferences(entity);
      var workstationBase = (IWorkstationBase) entity;
      Utils.PrepareWorkstationBaseDistributions(workstationBase, _distributionFactories);
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone((IInhomogeneousWorkstation) entity);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      var master = (IInhomogeneousWorkstation) entity;
      var clone = Clone(master);
      return clone;
    }

    protected override IEntity Create()
    {
      return new InhomogeneousWorkstation();
    }
  }
}