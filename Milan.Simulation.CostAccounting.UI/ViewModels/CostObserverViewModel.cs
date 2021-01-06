#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Emporer.Unit;
using Milan.JsonStore;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.Observers;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.CostAccounting.UI.ViewModels
{
  public class CostObserverViewModel : PropertyChangedBase, ICostObserverViewModel
  {
    private readonly ObservableCollection<string> _categories;
    private readonly IEnumerable<ICostObserverFactory> _costObserverFactories;
    private readonly CurrencyViewModel[] _currencies;

    private readonly IEntity _entity;
    private readonly EntityViewModel _entityViewModel;
    private readonly ProductTypeViewModel _nullProductType = new ProductTypeViewModel(new NullProductType());

    private readonly IEnumerable<ProductTypeViewModel> _productTypes;

    private readonly QuantityReference[] _quantityReferences = new[]
                                                                {
                                                                  QuantityReference.PerBatch, QuantityReference.PerProduct
                                                                };

    private readonly IJsonStore _store;

    private readonly TimeReference[] _timeReferences = new[]
                                                        {
                                                          TimeReference.Once, TimeReference.PerDay, TimeReference.PerHour, TimeReference.PerMinute,
                                                          TimeReference.PerSecond, TimeReference.PerMillisecond
                                                        };

    private double _amount;
    private double _lossRatio;

    private IEnumerable<ICostObserver> _availableProcesses;

    private string _category;
    private CurrencyViewModel _currency;
    private bool _isProductTypeRelated;
    private ICostObserver _observer;
    private ProductTypeViewModel _productType;
    private QuantityReference _quantityReference;
    private TimeReference _timeReference;

    public CostObserverViewModel(ICostObserver observer,
                                 IJsonStore store,
                                 IEnumerable<ICostObserverFactory> costObserverFactories,
                                 IEnumerable<ProductTypeViewModel> availableProductTypes,
                                 IEnumerable<IUnit> currencies,
                                 ObservableCollection<string> categories)
    {
      _observer = observer;
      _store = store;
      _costObserverFactories = costObserverFactories;

      _amount = observer.Amount;
      //hack: because converter can not be used
      _lossRatio = observer.LossRatio*100;
      _categories = categories;
      _category = observer.Category ?? string.Empty;
      _currencies = currencies.Select(x => new CurrencyViewModel(x))
                               .ToArray();

      // HACK: use a more explicit equality comparisson
      _currency = _currencies.Single(x => x.Model.Name == _observer.Currency.Name);

      _entity = observer.Entity;
      _entityViewModel = new EntityViewModel(_entity);

      UpdateAvailableProcesses();

      var productTypeDependent = observer as IProductRelated;
      if (productTypeDependent != null)
      {
        _isProductTypeRelated = true;
        _quantityReference = productTypeDependent.QuantityReference;
        _productTypes = new[]
                         {
                           _nullProductType
                         }.Concat(availableProductTypes.OrderBy(vm => vm.Name));

        _productType = productTypeDependent.IsProductTypeSpecific
                          ? _productTypes.Single(vm => vm.Model == productTypeDependent.ProductType)
                          : _nullProductType;
      }
      else
      {
        _isProductTypeRelated = false;
        _productTypes = new[]
                         {
                           _nullProductType // observer ist not product related, no specific product type can be chosen
                         };
        _productType = _nullProductType;
      }

      var timeReferenced = observer as ITimeReferenced;
      if (timeReferenced != null)
      {
        _timeReference = timeReferenced.TimeReference;
      }
      else
      {
        _timeReference = TimeReference.Once;
      }
    }

    public double LossRatio
    {
      get
      {
        return _lossRatio;
      }
      set
      {
        if (_lossRatio == value)
        {
          return;
        }
        _lossRatio = value;
        //hack: because converter can not be used
        _observer.LossRatio = value/100;
        NotifyOfPropertyChange(() => LossRatio);
      }
    }

    public double Amount
    {
      get { return _amount; }
      set
      {
        _amount = value;
        _observer.Amount = value;
        NotifyOfPropertyChange(() => Amount);
      }
    }

    public ObservableCollection<string> Categories
    {
      get { return _categories; }
    }

    public string Category
    {
      get { return _category; }
      set
      {
        if (_category == value)
        {
          return;
        }
        _category = value;
        _observer.Category = value;
        NotifyOfPropertyChange(() => Category);
      }
    }

    public IEnumerable<CurrencyViewModel> Currencies
    {
      get { return _currencies; }
    }

    public CurrencyViewModel Currency
    {
      get { return _currency; }
      set
      {
        if (_currency == value)
        {
          return;
        }
        _currency = value;
        _observer.Currency = value.Model;
        NotifyOfPropertyChange(() => Currency);
      }
    }

    public EntityViewModel Entity
    {
      get { return _entityViewModel; }
    }

    public bool IsProductTypeRelated
    {
      get { return _isProductTypeRelated; }
    }

    public ICostObserver Process
    {
      get { return _observer; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException();
        }

        if (_observer == value)
        {
          return;
        }

        var previousValue = _observer;
        SetProperties(value);
        _observer = value;

        _entity.Model.Remove(previousValue);
        _entity.Model.Add(_observer);
        //TODO: why do this?
        _store.Add(_entity);

        _isProductTypeRelated = _observer is IProductRelated;
        UpdateAvailableProcesses();
        NotifyOfPropertyChange(() => Process);
        NotifyOfPropertyChange(() => IsProductTypeRelated);
      }
    }

    public IEnumerable<ICostObserver> Processes
    {
      get { return _availableProcesses; }
    }

    public ProductTypeViewModel ProductType
    {
      get { return _productType; }
      set
      {
        if (_productType == value)
        {
          return;
        }
        _productType = value;

        var productRelated = ((IProductRelated) _observer);

        if (productRelated == null)
        {
          return; // model is not product type related, don't set
        }

        if (_productType == _nullProductType)
        {
          productRelated.ProductType = null;
          productRelated.IsProductTypeSpecific = false;
        }
        else
        {
          productRelated.ProductType = _productType.Model;
          productRelated.IsProductTypeSpecific = true;
        }

        NotifyOfPropertyChange(() => ProductType);
        NotifyOfPropertyChange(() => IsProductTypeRelated);
      }
    }

    public IEnumerable<ProductTypeViewModel> ProductTypes
    {
      get { return _productTypes; }
    }

    public QuantityReference QuantityReference
    {
      get { return _quantityReference; }
      set
      {
        if (_quantityReference == value)
        {
          return;
        }
        _quantityReference = value;
        ((IProductRelated) _observer).QuantityReference = value;

        NotifyOfPropertyChange(() => QuantityReference);
      }
    }


    public IEnumerable<QuantityReference> QuantityReferences
    {
      get { return _quantityReferences; }
    }

    public TimeReference TimeReference
    {
      get { return _timeReference; }
      set
      {
        if (_timeReference == value)
        {
          return;
        }
        _timeReference = value;
        ((ITimeReferenced) _observer).TimeReference = value;
        NotifyOfPropertyChange(() => TimeReference);
      }
    }

    public IEnumerable<TimeReference> TimeReferences
    {
      get { return _timeReferences; }
    }

    public object Model
    {
      get { return _observer; }
    }


    public void UpdateCategory()
    {
      _observer.Category = _category;

      if (!_categories.Contains(_category))
      {
        _categories.Add(_category);
      }
    }

    private void SetProperties(ICostObserver observer)
    {
      observer.Entity = _entity;
      observer.Amount = _amount;
      observer.LossRatio = _lossRatio;
      observer.Currency = _currency.Model;

      var timeReferenced = observer as ITimeReferenced;
      if (timeReferenced != null)
      {
        timeReferenced.TimeReference = _timeReference;
      }

      var productTypeRelated = observer as IProductRelated;
      if (productTypeRelated != null)
      {
        productTypeRelated.IsProductTypeSpecific = _isProductTypeRelated;
        productTypeRelated.ProductType = _productType.Model;
        productTypeRelated.QuantityReference = _quantityReference;
      }
    }

    private void UpdateAvailableProcesses()
    {
      // create all observers for current entity, exchange selected observer type
      //todo: create missing observer only on demand when it is selected
      _availableProcesses = _costObserverFactories.Where(cof => cof.CanHandle(_entity)).Select(cof => cof.Create())
                                                    .Where(o => o.GetType() != _observer.GetType())
                                                    .Concat(new[]
                                                            {
                                                              _observer
                                                            });
      NotifyOfPropertyChange(() => Processes);
    }
  }
}