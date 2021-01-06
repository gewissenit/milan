using System.Collections.Generic;
using System.ComponentModel.Composition;
using Milan.Simulation.Factories;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof(IEntityFactory))]
  internal class InfluenceFactory : EntityFactory
  {
    [ImportingConstructor]
    public InfluenceFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions)
      : base("Influence", additionalEntityDuplicationActions)
    {
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is Influence;
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Create();
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      return Create();
    }

    protected override IEntity Create()
    {
      return new Influence();
    }
  }
}