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
  [JsonObject(MemberSerialization.OptIn)]
  public class ProductTypeProcessCostObserver<TEntity, TStartEvent, TEndEvent> : ProcessCostObserver<TEntity, TStartEvent, TEndEvent>, IProductTypeProcessCostObserver<TEntity>
    where TStartEvent : class, ISimulationEvent
    where TEndEvent : class, IRelatedEvent
    where TEntity : class, IEntity
  {
    public ProductTypeProcessCostObserver(string name)
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
      var startEvent = endEvent.RelatedEvent as TStartEvent;
      if (startEvent == null)
      {
        return;
      }

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

      var duration = endEvent.ScheduledTime - endEvent.RelatedEvent.ScheduledTime;
      var value = ApplyTimeReference(Amount, duration);

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