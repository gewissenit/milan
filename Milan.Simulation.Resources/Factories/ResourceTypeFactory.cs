#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation.Factories;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class ResourceTypeFactory : EntityFactory
  {
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;

    [ImportingConstructor]
    public ResourceTypeFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> duplicationAction,
                               [ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories)
      : base("Resource Type", duplicationAction)
    {
      _distributionFactories = distributionFactories;
      
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is ResourceType;
    }
    
    protected override IEntity Copy(IEntity entity)
    {
      var rt = (IResourceType) entity;
      var clone = (IResourceType) Create();
      clone.MaintenanceDuration = _distributionFactories.Single(dc => dc.CanHandle(rt.MaintenanceDuration))
                                                        .DuplicateConfiguration(rt.MaintenanceDuration);
      clone.UsageAmount = _distributionFactories.Single(dc => dc.CanHandle(rt.UsageAmount))
                                                        .DuplicateConfiguration(rt.UsageAmount);
      foreach (var influence in rt.Influences)
      {
        clone.Add(influence);
      }

      return clone;
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      var rt = (IResourceType) entity;
      var clone = (IResourceType) Create();
      clone.MaintenanceDuration = rt.MaintenanceDuration;
      clone.UsageAmount = rt.UsageAmount;
      foreach (var influence in rt.Influences)
      {
        clone.Add(influence);
      }
      return clone;
    }

    protected override IEntity Create()
    {
      return new ResourceType();
    }
  }
}