#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation.CostAccounting
{
  public class ProductTypeEventCostObserver<TEntity, TEndEvent> : EventCostObserver<TEntity, TEndEvent>, IProductTypeCostObserver<TEntity>
    where TEndEvent : class, ISimulationEvent
    where TEntity : class, IEntity
  {
    protected ProductTypeEventCostObserver(string name)
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

    protected override void OnEntityEventOccurred(TEndEvent endEvent)
    {
      var productRelatedEndEvent = endEvent as IProductsRelatedEvent;
      if (productRelatedEndEvent == null)
      {
        return;
      }

      var products = productRelatedEndEvent.Products.ToArray();
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

      var value = Amount;
      var productValue = QuantityReference == QuantityReference.PerProduct
                           ? value
                           : value / productCount;

      foreach (var product in products)
      {
        CreateBalancePosition(endEvent, Entity, Currency, CurrentExperiment, productValue, LossRatio, Name, Category, product.ProductType, product.Id);
      }
    }
  }
}