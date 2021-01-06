#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Events;
using EcoFactory.Components.States;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class EntryPoint : StationaryElement, IEntryPoint
  {
    private IList<ArrivalProcess> _arrivalStates;
    private Dictionary<IProductType, IRealDistribution> _arrivalsDictionary;
    private Dictionary<IProductType, IRealDistribution> _batchesDictionary;
    private bool _isOff;

    [JsonProperty]
    public bool IsWorkingTimeDependent
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration BatchSize
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    public override bool CanConnectToSource(INode<IConnection> node)
    {
      return false;
    }

    public IRealDistribution BatchSizeDistribution { get; set; }

    public void OnWorkingTimeStarted()
    {
      if (!IsWorkingTimeDependent)
      {
        return;
      }
      if (!_isOff)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      _isOff = false;
      if (!_arrivalStates.Any())
      {
        foreach (var productType in ArrivalsDictionary.Keys)
        {
          CreateArrival(productType);
        }
      }
      else
      {
        foreach (var arrivalState in _arrivalStates)
        {
          arrivalState.Resume();
        }
      }
    }

    public void OnWorkingTimeEnded()
    {
      if (!IsWorkingTimeDependent)
      {
        return;
      }
      if (_isOff)
      {
        throw new InvalidOperationException("This should not occur!");
      }
      _isOff = true;
      foreach (var arrivalState in _arrivalStates)
      {
        arrivalState.Suspend();
      }
    }

    #region ArrivalOccurrences

    [JsonProperty]
    private readonly IList<IProductTypeDistribution> _ArrivalOccurrences = new List<IProductTypeDistribution>();

    public IEnumerable<IProductTypeDistribution> ArrivalOccurrences
    {
      get { return _ArrivalOccurrences; }
    }


    public IDictionary<IProductType, IRealDistribution> ArrivalsDictionary
    {
      get
      {
        if (_arrivalsDictionary == null)
        {
          _arrivalsDictionary = new Dictionary<IProductType, IRealDistribution>();
        }
        return _arrivalsDictionary;
      }
    }

    public void AddArrival(IProductTypeDistribution productTypeDistribution)
    {
      if (_ArrivalOccurrences.Any(cm => cm.ProductType == productTypeDistribution.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _ArrivalOccurrences.Add(productTypeDistribution);
    }


    public void RemoveArrival(IProductTypeDistribution productTypeDistribution)
    {
      if (!_ArrivalOccurrences.Contains(productTypeDistribution))
      {
        throw new InvalidOperationException();
      }
      _ArrivalOccurrences.Remove(productTypeDistribution);
    }

    #endregion

    #region BatchSizes

    [JsonProperty]
    private readonly IList<IProductTypeDistribution> _BatchSizes = new List<IProductTypeDistribution>();

    public IEnumerable<IProductTypeDistribution> BatchSizes
    {
      get { return _BatchSizes; }
    }

    public IDictionary<IProductType, IRealDistribution> BatchesDictionary
    {
      get
      {
        if (_batchesDictionary == null)
        {
          _batchesDictionary = new Dictionary<IProductType, IRealDistribution>();
        }
        return _batchesDictionary;
      }
    }


    public void AddBatchSize(IProductTypeDistribution productTypeDistribution)
    {
      if (_BatchSizes.Any(cm => cm.ProductType == productTypeDistribution.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _BatchSizes.Add(productTypeDistribution);
    }


    public void RemoveBatchSize(IProductTypeDistribution productTypeDistribution)
    {
      if (!_BatchSizes.Contains(productTypeDistribution))
      {
        throw new InvalidOperationException("The given productTypeDistribution does not exist.");
      }
      _BatchSizes.Remove(productTypeDistribution);
    }

    #endregion

    protected override IEnumerable<Product> GetResidingProducts()
    {
      return new Product[0];
    }

    private void CreateArrival(IProductType productType)
    {
      var batchSize = BatchesDictionary.ContainsKey(productType)
                        ? BatchesDictionary[productType].GetSample()
                        : BatchSizeDistribution.GetSample();
      batchSize = Math.Round(batchSize);
      var arrival = new ArrivalProcess(this, batchSize, productType, ArrivalsDictionary[productType], CreateMissedOrEntered);
      _arrivalStates.Add(arrival);
      arrival.Start();
    }

    private void CreateMissedOrEntered(ISimulationEvent schedulable, ArrivalProcess state)
    {
      _arrivalStates.Remove(state);
      var productsArrivedEvent = (ProductsArrivedEvent) schedulable;
      var products = productsArrivedEvent.Products;
      IProductType productType = null;
      foreach (var product in products)
      {
        productType = product.ProductType;

        /*HACK: product creation happened at a point in time where its real life time in the model wasn't determined.
         * So we have to reset the timestamp here. */
        product.TimeStamp = CurrentExperiment.CurrentTime;
        product.EntryPoint = this;
        product.CurrentLocation = this;

        if (_productSender.CanTransmit(product))
        {
          var entered = new ThroughputStartEvent(this,
                                                 new[]
                                                 {
                                                   product
                                                 });
          entered.Schedule(0);
          _productSender.Transmit(product);
        }
        else
        {
          var missed = new ProductMissedEvent(this,
                                              new[]
                                              {
                                                product
                                              });
          missed.Schedule(0);
        }
      }
      CreateArrival(productType);
    }
    
    //TODO: extract to validation
    private void CheckBatchSizes()
    {
      if (!BatchesDictionary.Any() &&
          BatchSizeDistribution == null)
      {
        throw new ModelConfigurationException(Model,
                                              this, $"Batch sizes in workstation {Name} are not well defined. Please add a default distribution or add at least one product type specific one.",
                                              "BatchSizes");
      }
    }

    #region Overrides

    public override void Initialize()
    {
      base.Initialize();
      _arrivalStates = new List<ArrivalProcess>();

      CheckBatchSizes();
      if (IsWorkingTimeDependent)
      {
        _isOff = true;
      }
      else
      {
        foreach (var productType in ArrivalsDictionary.Keys)
        {
          CreateArrival(productType);
        }
      }
    }


    public override bool IsAvailable(Product product)
    {
      return false;
    }

    public override void Reset()
    {
      _isOff = false;
      ArrivalsDictionary.Clear();
      BatchSizeDistribution = null;
      BatchesDictionary.Clear();
      _arrivalStates = null;
      base.Reset();
    }

    #endregion
  }
}