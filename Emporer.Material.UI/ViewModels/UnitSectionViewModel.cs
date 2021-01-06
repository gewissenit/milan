using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Emporer.Unit;
using Emporer.Unit.Factories;

namespace Emporer.Material.UI.ViewModels
{
  public class UnitSectionViewModel : Screen
  {
    private readonly IMaterial _model;
    private readonly IUnitFactory _unitFactory;
    private readonly ObservableCollection<IUnit> _units = new ObservableCollection<IUnit>();
    private bool _isInEditMode;

    public UnitSectionViewModel(IMaterial model,
                                IUnitFactory unitFactory,
                                IEnumerable<IUnit> standardUnits)
    {
      DisplayName = "unit";
      _model = model;
      _unitFactory = unitFactory;
      _units = new ObservableCollection<IUnit>(standardUnits.Where(u => u.Dimension != "Currency" && u.Dimension != "Time" && u.Dimension != "Temperature"));

      ReferencedUnits = Units.Where(u => u.IsBasicUnit)
                             .ToArray();
      _isInEditMode = OwnUnit != null;
    }


    public IEnumerable<IUnit> Units
    {
      get { return _units; }
    }

    public IEnumerable<IUnit> ReferencedUnits { get; private set; }

    public IUnit DisplayUnit
    {
      get { return _model.DisplayUnit; }
      set
      {
        if (_model.DisplayUnit == value)
        {
          return;
        }
        _model.DisplayUnit = value;
        NotifyOfPropertyChange(() => DisplayUnit);
      }
    }

    public IUnit OwnUnit
    {
      get { return _model.OwnUnit; }
      set
      {
        if (_model.OwnUnit == value)
        {
          return;
        }
        _model.OwnUnit = value;
        NotifyOfPropertyChange(() => OwnUnit);
        NotifyOfPropertyChange(() => OwnUnitName);
        NotifyOfPropertyChange(() => OwnUnitCoefficient);
        NotifyOfPropertyChange(() => OwnUnitSymbol);
        NotifyOfPropertyChange(() => OwnUnitReferencedUnit);
        NotifyOfPropertyChange(() => Conversion);
      }
    }

    public string OwnUnitName
    {
      get
      {
        if (OwnUnit == null)
        {
          return string.Empty;
        }
        return OwnUnit.Name;
      }
      set
      {
        if (OwnUnit == null ||
            OwnUnit.Name == value)
        {
          return;
        }
        OwnUnit.Name = value;
        NotifyOfPropertyChange(() => OwnUnitName);
      }
    }

    public double OwnUnitCoefficient
    {
      get
      {
        if (OwnUnit == null)
        {
          return 0;
        }
        return OwnUnit.Coefficient;
      }
      set
      {
        if (OwnUnit == null ||
            OwnUnit.Coefficient == value)
        {
          return;
        }
        OwnUnit.Coefficient = value;
        NotifyOfPropertyChange(() => OwnUnitCoefficient);
        NotifyOfPropertyChange(() => Conversion);
      }
    }

    public string OwnUnitSymbol
    {
      get
      {
        if (OwnUnit == null)
        {
          return string.Empty;
        }
        return OwnUnit.Symbol;
      }
      set
      {
        if (OwnUnit == null ||
            OwnUnit.Symbol == value)
        {
          return;
        }
        OwnUnit.Symbol = value;
        NotifyOfPropertyChange(() => OwnUnitSymbol);
        NotifyOfPropertyChange(() => Conversion);
      }
    }

    public IUnit OwnUnitReferencedUnit
    {
      get
      {
        if (OwnUnit == null)
        {
          return null;
        }
        return OwnUnit.ReferencedUnit;
      }
      set
      {
        if (OwnUnit == null ||
            OwnUnit.ReferencedUnit == value)
        {
          return;
        }
        OwnUnit.ReferencedUnit = value;
        NotifyOfPropertyChange(() => OwnUnitReferencedUnit);
        NotifyOfPropertyChange(() => Conversion);
      }
    }

    public string Conversion
    {
      get
      {
        if (OwnUnitReferencedUnit == null)
        {
          return string.Empty;
        }
        return string.Format("1 {0} = {1} {2}\n1 {2} = {3} {0}",
                             OwnUnitSymbol,
                             OwnUnit.ToBaseUnit(1),
                             OwnUnit.BasicUnit.Symbol,
                             1 / OwnUnit.ToBaseUnit(1));
      }
    }

    public bool IsInEditMode
    {
      get { return _isInEditMode; }
      set
      {
        if (_isInEditMode == value)
        {
          return;
        }
        if (value)
        {
          OwnUnit = _unitFactory.Create();
          DisplayUnit = OwnUnit;
        }
        else
        {
          OwnUnit = null;
          DisplayUnit = _units.First();
        }

        _isInEditMode = value;
        NotifyOfPropertyChange(() => IsInEditMode);
      }
    }
  }
}