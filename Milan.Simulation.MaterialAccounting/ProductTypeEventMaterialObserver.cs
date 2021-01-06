#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation.MaterialAccounting
{
  public class ProductTypeEventMaterialObserver<TEntity, TObservedEvent> : EventMaterialObserver<TEntity, TObservedEvent>, IProductRelated
    where TEntity : class, IEntity
    where TObservedEvent : class, ISimulationEvent
  {
    protected ProductTypeEventMaterialObserver(string name)
      : base(name)
    {
    }

    [JsonProperty]
    public bool IsProductTypeSpecific
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IProductType ProductType
    {
      get { return Get<IProductType>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public QuantityReference QuantityReference
    {
      get { return Get<QuantityReference>(); }
      set { Set(value); }
    }

    protected override void OnEntityEventOccurred(TObservedEvent endEvent)
    {
      var productReleatedEndEvent = endEvent as IProductsRelatedEvent;
      if (productReleatedEndEvent == null)
      {
        return;
      }

      var products = productReleatedEndEvent.Products.ToArray();
      if (IsProductTypeSpecific)
      {
        products = products.Where(p => p.ProductType == ProductType)
                           .ToArray();
      }

      var productCount = products.Count();
      if (productCount == 0)
      {
        return;
      }

      var value = Unit.ToBaseUnit(Amount);
      var productValue = QuantityReference == QuantityReference.PerProduct
                           ? value
                           : value / productCount;
      foreach (var product in products)
      {
        CreateBalancePosition(endEvent, Entity, Material, CurrentExperiment, productValue, LossRatio, Name, Category, product.ProductType, product.Id);
      }
    }
  }
}