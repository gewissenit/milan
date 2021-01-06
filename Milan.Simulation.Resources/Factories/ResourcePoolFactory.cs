using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation.Factories;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class ResourcePoolFactory : EntityFactory
  {
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;
    private readonly IResourceTypeAmountFactory _resourceTypeAmountFactory;

    [ImportingConstructor]
    public ResourcePoolFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> duplicationAction,
                               [Import] IResourceTypeAmountFactory resourceTypeAmountFactory,
                               [ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories)
      : base("Resource Pool", duplicationAction)
    {
      _resourceTypeAmountFactory = resourceTypeAmountFactory;
      _distributionFactories = distributionFactories;
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is ResourcePool;
    }

    private IEntity Clone(IEntity entity)
    {
      var master = (IResourcePool) entity;
      var clone = new ResourcePool();

      foreach (var resourceTypeAmount in master.Resources)
      {
        clone.Add(_resourceTypeAmountFactory.Duplicate(resourceTypeAmount));
      }
      return clone;
    }

    public override void ResolveReferences(IEntity entity)
    {
      var rp = (IResourcePool) entity;
      foreach (var resourceTypeAmount in rp.Resources)
      {
        var rt = rp.Model.Entities.OfType<IResourceType>()
                   .Single(e => e.Id == resourceTypeAmount.ResourceType.Id);
        resourceTypeAmount.ResourceType = rt;
      }
    }

    public override void Prepare(IEntity entity)
    {
      ResolveReferences(entity);
      var rp = (IResourcePool) entity;
      foreach (var resourceTypeAmount in rp.Resources)
      {
        for (var i = 0; i < resourceTypeAmount.Amount; i++)
        {
          var resource = new Resource(resourceTypeAmount.ResourceType, rp, () =>
          {
            //HACK: getting current time fails once because scheduler isn't there when it is called the first time
            if (rp.CurrentExperiment.Scheduler==null)
            {
              return 0;
            }
            return rp.CurrentExperiment.CurrentTime;
          }
          );
          if (resourceTypeAmount.ResourceType.MaintenanceDuration != null)
          {
            resource.MaintenanceDuration = Utils.CreateAndSetupDistribution(resourceTypeAmount.ResourceType.MaintenanceDuration,
                                                                            _distributionFactories,
                                                                            resourceTypeAmount.ResourceType);
          }
          if (resourceTypeAmount.ResourceType.UsageAmount != null)
          {
            resource.UsageAmount = Utils.CreateAndSetupDistribution(resourceTypeAmount.ResourceType.UsageAmount,
                                                                    _distributionFactories,
                                                                    resourceTypeAmount.ResourceType);
          }
          rp.AvailableResources.Add(resource);
        }
      }
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone(entity);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      return Clone(entity);
    }

    protected override IEntity Create()
    {
      return new ResourcePool();
    }
  }
}