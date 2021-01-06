#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class ProductTypeFactory : EntityFactory
  {
    [ImportingConstructor]
    public ProductTypeFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions)
      : base("Product Type", additionalEntityDuplicationActions)
    {
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is ProductType;
    }

    private IEntity Clone(IEntity entity)
    {
      var master = (IProductType) entity;
      var clone = new ProductType
                  {
                    IconId = master.IconId
                  };
      return clone;
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
      return new ProductType();
    }
  }
}