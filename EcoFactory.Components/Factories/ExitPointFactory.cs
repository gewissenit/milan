#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.Factories;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class ExitPointFactory : EntityFactory
  {
    [ImportingConstructor]
    public ExitPointFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions)
      : base("Exit Point", additionalEntityDuplicationActions)
    {
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is ExitPoint;
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
      return new ExitPoint();
    }
  }
}