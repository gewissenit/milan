#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation;
using Milan.Simulation.Accessories;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Storage : StationaryElement, IStorage
  {
    [JsonProperty]
    private readonly IList<IProductTypeAmount> _ProductTypeCapacities = new List<IProductTypeAmount>();

    private FiniteQueueCluster<Product, IProductType> _queuedProducts = new FiniteQueueCluster<Product, IProductType>();

    public Storage()
    {
      //todo: move this to factory
      Capacity = 1;
    }

    public int Count
    {
      get { return _queuedProducts.GetCount(); }
    }

    public IEnumerable<IProductTypeAmount> ProductTypeCapacities
    {
      get { return _ProductTypeCapacities; }
    }

    public IEnumerable<IProductTypeAmount> ProductTypeCounts
    {
      get { return _queuedProducts.Categories.Select(productType => new ProductTypeAmount(productType, _queuedProducts.GetCount(productType))); }
    }

    public int GetCount(IProductType productType)
    {
      if (_queuedProducts.Categories.Contains(productType))
      {
        return _queuedProducts.GetCount(productType);
      }
      return 0;
    }

    public void AddProductTypeCapacity(IProductTypeAmount productTypeCapacity)
    {
      if (_ProductTypeCapacities.Any(cm => cm.ProductType == productTypeCapacity.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _ProductTypeCapacities.Add(productTypeCapacity);
    }

    public void RemoveProductTypeCapacity(IProductTypeAmount productTypeCapacity)
    {
      if (!_ProductTypeCapacities.Contains(productTypeCapacity))
      {
        throw new InvalidOperationException();
      }
      _ProductTypeCapacities.Remove(productTypeCapacity);
    }

    public override void Initialize()
    {
      base.Initialize();
      if (!HasLimitedCapacity)
      {
        return;
      }
      CheckCapacities();
      if (HasCapacityPerProductType)
      {
        foreach (var capacityForProductType in ProductTypeCapacities)
        {
          _queuedProducts.SetCapacity(capacityForProductType.Amount, capacityForProductType.ProductType);
        }
      }
      else
      {
        _queuedProducts.OverallCapacity = Capacity;
      }
    }

    public override void Receive(Product product)
    {
      base.Receive(product);
      if (!_productSender.CanTransmit(product))
      {
        _queuedProducts.Enqueue(product, product.ProductType);
      }
      else
      {
        Send(product);
      }
    }

    public override bool IsAvailable(Product product)
    {
      return !_queuedProducts.IsFull(product.ProductType);
    }

    public override void Reset()
    {
      _queuedProducts.Clear();
      base.Reset();
    }

    [JsonProperty]
    public bool HasCapacityPerProductType
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool HasLimitedCapacity
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Capacity
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    //TODO: extract to validation
    private void CheckCapacities()
    {
      if (HasCapacityPerProductType)
      {
        if (!ProductTypeCapacities.Any())
        {
          throw new ModelConfigurationException(Model,
                                                this, $"The product type dependent capacities in storage {Name} are not well defined. If it has a product type dependent limited capacity specific capacity should be added.",
                                                "ProductTypeCapacities");
        }
      }
      else if (Capacity < 1)
      {
        throw new ModelConfigurationException(Model,
                                              this, $"The Capacity in storage {Name} is not well defined. If it has a limited capacity the capacity must be higher then 0.",
                                              "Capacity");
      }
    }

    protected override IEnumerable<Product> GetResidingProducts()
    {
      return _queuedProducts.StoredItems;
    }

    private void TrySend()
    {
      if (_queuedProducts.IsEmpty())
      {
        return;
      }

      while (!_queuedProducts.IsEmpty() &&
             _productSender.CanTransmit(_queuedProducts.Peek()))
      {
        var product = _queuedProducts.Dequeue();
        Send(product);
      }

      if (_queuedProducts.IsEmpty())
      {
        return;
      }

      SendFifo(_queuedProducts.Peek());
    }

    private void SendFifo(Product prevProduct)
    {
      Product nextProduct;
      if (_queuedProducts.TryGetNextEntityOfDifferentCategory(prevProduct, out nextProduct))
      {
        if (_productSender.CanTransmit(nextProduct))
        {
          _queuedProducts.Remove(nextProduct);
          Send(nextProduct);
          TrySend();
        }
        else
        {
          SendFifo(nextProduct);
        }
      }
    }

    protected override void OnGotAvailable(object sender, EventArgs e)
    {
      if (_initialized)
      {
        TrySend();
      }
    }
  }
}