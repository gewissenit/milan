#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Milan.Simulation;
using Milan.Simulation.Factories;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class StorageFactory : EntityFactory
  {
    private readonly IConnectionFactory _connectionFactory;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;

    [ImportingConstructor]
    public StorageFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions,
                          [Import] IConnectionFactory connectionFactory,
                          [Import] IProductTypeAmountFactory productTypeAmountFactory)
      : base("Storage", additionalEntityDuplicationActions)
    {
      _connectionFactory = connectionFactory;
      _productTypeAmountFactory = productTypeAmountFactory;
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is Storage;
    }

    private Storage Clone(IStorage master)
    {
      var clone = new Storage
                  {
                    HasCapacityPerProductType = master.HasCapacityPerProductType,
                    HasLimitedCapacity = master.HasLimitedCapacity,
                    Capacity = master.Capacity
                  };

      foreach (var productTypeCapacity in master.ProductTypeCapacities)
      {
        clone.AddProductTypeCapacity(_productTypeAmountFactory.Duplicate(productTypeCapacity));
      }
      
      Milan.Simulation.Utils.CloneStationaryElement(master, clone, _connectionFactory);

      return clone;
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone((IStorage) entity);
    }

    public override void ResolveReferences(IEntity entity)
    {
      var st = (IStorage) entity;
      Milan.Simulation.Utils.PrepareStationaryElement(st);
      foreach (var capacityForProductType in st.ProductTypeCapacities)
      {
        var modelProductTypes = st.Model.Entities.OfType<IProductType>()
                                  .ToArray();
        var productType = modelProductTypes.Single(e => e.Id == capacityForProductType.ProductType.Id);
        capacityForProductType.ProductType = productType;
      }
    }

    public override void Prepare(IEntity entity)
    {
      ResolveReferences(entity);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      var master = (IStorage) entity;
      var clone = Clone(master);
      return clone;
    }

    protected override IEntity Create()
    {
      return new Storage();
    }
  }
}