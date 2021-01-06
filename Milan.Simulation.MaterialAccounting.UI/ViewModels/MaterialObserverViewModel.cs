#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using Emporer.Material;
using Emporer.Material.UI.ViewModels;
using Emporer.Unit;
using Emporer.Unit.UI.ViewModels;
using Milan.JsonStore;
using Milan.Simulation.MaterialAccounting.Factories;
using Milan.Simulation.Observers;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.MaterialAccounting.UI.ViewModels
{
  public class MaterialObserverViewModel : PropertyChangedBase, IMaterialObserverViewModel
  {
    //private readonly IEnumerable<BalanceDirection> _balanceSides = Enum.GetValues(typeof (BalanceDirection))
    //                                                                   .OfType<BalanceDirection>()
    //                                                                   .ToArray();

    private readonly ObservableCollection<string> _categories;
    private readonly IEntity _entity;
    private readonly EntityViewModel _entityViewModel;
    private readonly IEnumerable<IMaterialObserverFactory> _materialObserverFactories;
    private readonly ProductTypeViewModel _nullProductType = new ProductTypeViewModel(new NullProductType());

    private readonly IEnumerable<ProductTypeViewModel> _productTypes;

    private readonly IEnumerable<QuantityReference> _quantityReferences = Enum.GetValues(typeof (QuantityReference))
                                                                              .OfType<QuantityReference>()
                                                                              .ToArray();

    private readonly IJsonStore _store;

    private readonly IEnumerable<TimeReference> _timeReferences = Enum.GetValues(typeof (TimeReference))
                                                                      .OfType<TimeReference>()
                                                                      .ToArray();

    private readonly IEnumerable<IUnit> _units;

    private double _amount;
    private IEnumerable<MaterialViewModel> _availableMaterials;
    private IEnumerable<IMaterialObserver> _availableProcesses;
    private UnitViewModel[] _availableUnits;
    //private BalanceDirection _balanceSide;
    private double _lossRatio;
    private string _category;
    private bool _isProductTypeRelated;
    private MaterialViewModel _material;
    private IMaterialObserver _observer;
    private ProductTypeViewModel _productType;
    private QuantityReference _quantityReference;
    private TimeReference _timeReference;
    private UnitViewModel _unit;

    public MaterialObserverViewModel(IMaterialObserver observer,
                                     IJsonStore store,
                                     IEnumerable<IMaterialObserverFactory> materialObserverFactories,
                                     IEnumerable<ProductTypeViewModel> availableProductTypes,
                                     ObservableCollection<string> categories,
                                     IEnumerable<IUnit> units)
    {
      _observer = observer;
      _store = store;
      _materialObserverFactories = materialObserverFactories;
      _lossRatio = observer.LossRatio*100;
      _amount = observer.Amount;
      _categories = categories;
      Category = observer.Category ?? string.Empty;

      _units = units.Where(u => u.Dimension != "Currency" && u.Dimension != "Time" && u.Dimension != "Temperature")
                    .ToArray();
      Material = new MaterialViewModel(_observer.Material);

      _entity = observer.Entity;

      if (_entity == null)
      {
        throw new ArgumentException();
      }

      _entityViewModel = new EntityViewModel(_entity);

      UpdateAvailableProcesses();
      UpdateAvailableMaterials();
      UpdateAvailableUnits();

      var productTypeDependent = observer as IProductRelated;
      if (productTypeDependent != null)
      {
        _isProductTypeRelated = true;
        QuantityReference = productTypeDependent.QuantityReference;
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

    public double Amount
    {
      get { return _amount; }
      set
      {
        if (_amount == value)
        {
          return;
        }
        _amount = value;
        _observer.Amount = value;
        NotifyOfPropertyChange(() => Amount);
      }
    }

    public IEnumerable<UnitViewModel> Units
    {
      get { return _availableUnits; }
    }

    public UnitViewModel Unit
    {
      get { return _unit; }
      set
      {
        if (_unit == value)
        {
          return;
        }
        _unit = value;
        _observer.Unit = value.Model;
        NotifyOfPropertyChange(() => Unit);
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
        _observer.LossRatio = value/100;
        NotifyOfPropertyChange(() => LossRatio);
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
        if (value == null)
        {
          throw new ArgumentNullException();
        }
        if (_category == value)
        {
          return;
        }
        _category = value;
        _observer.Category = value;
        NotifyOfPropertyChange(() => Category);
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

    public MaterialViewModel Material
    {
      get { return _material; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException();
        }
        if (_material == value)
        {
          return;
        }
        if (_material != null)
        {
          _material.Model.PropertyChanged -= MaterialOnPropertyChanged;
        }
        _material = value;
        _material.Model.PropertyChanged += MaterialOnPropertyChanged;
        _observer.Material = _material.Model;

        UpdateAvailableMaterials();
        UpdateAvailableUnits();
        NotifyOfPropertyChange(() => Material);
      }
    }

    public IEnumerable<MaterialViewModel> Materials
    {
      get { return _availableMaterials; }
    }


    public IMaterialObserver Process
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

    public IEnumerable<IMaterialObserver> Processes
    {
      get { return _availableProcesses; }
    }

    public ProductTypeViewModel ProductType
    {
      get { return _productType; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException();
        }

        if (_productType == value)
        {
          return;
        }

        var productRelated = (IProductRelated) _observer;
        _productType = value;

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

    private void MaterialOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      if (propertyChangedEventArgs.PropertyName == "DisplayUnit")
      {
        UpdateAvailableUnits();
      }
    }

    private void UpdateAvailableUnits()
    {
      if (_observer.Material.DisplayUnit == null)
      {
        _availableUnits = new UnitViewModel[0];
        Unit = null;
        return;
      }
      if (_observer.Unit != null &&
          (!_observer.Unit.IsConvertibleTo(_observer.Material.DisplayUnit) || (_observer.Unit != _observer.Material.DisplayUnit && !_observer.Unit.IsReadonly)))
      {
        _observer.Unit = null;
      }
      _availableUnits = _units.Where(u => u.IsConvertibleTo(_observer.Material.DisplayUnit))
                              .Select(x => new UnitViewModel(x))
                              .ToArray();
      if (!_observer.Material.DisplayUnit.IsReadonly)
      {
        _availableUnits = _availableUnits.Concat(new[]
                                                 {
                                                   new UnitViewModel(_observer.Material.DisplayUnit)
                                                 })
                                         .ToArray();
      }
      var unit = _observer.Unit ?? _observer.Material.DisplayUnit;
      Unit = _availableUnits.Single(x => x.Model == unit);
      NotifyOfPropertyChange(() => Units);
    }

    public void UpdateCategory()
    {
      if (!_categories.Contains(_category))
      {
        _categories.Add(_category);
      }
    }

    private void SetProperties(IMaterialObserver observer)
    {
      observer.Entity = _entity;
      observer.Amount = _amount;
      observer.LossRatio = _lossRatio;
      observer.Material = _material.Model;
      if(_unit != null)
      {
        observer.Unit = _unit.Model;
      }
      observer.Category = _category;
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

    private void UpdateAvailableMaterials()
    {
      _availableMaterials = _store.Content.OfType<IMaterial>()
                                  .Where(x => x != _material.Model)
                                  .Select(x => new MaterialViewModel(x))
                                  .Concat(new[]
                                          {
                                            _material
                                          });
      NotifyOfPropertyChange(() => Materials);
    }

    private void UpdateAvailableProcesses()
    {
      // create all observers for current entity, exchange selected observer type
      //todo: create missing observer only on demand when it is selected
      _availableProcesses = _materialObserverFactories.Where(mof => mof.CanHandle(_entity))
                                                      .Select(mof => mof.Create())
                                                      .Where(o => o.GetType() != _observer.GetType())
                                                      .Concat(new[]
                                                              {
                                                                _observer
                                                              });
      NotifyOfPropertyChange(() => Processes);
    }
  }
}