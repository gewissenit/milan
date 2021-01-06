using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Emporer.Unit;

namespace Emporer.Material.UI.ViewModels
{
  public class CostsSectionViewModel : Screen
  {
    private readonly IMaterial _model;

    public CostsSectionViewModel(IMaterial model,
                                 IEnumerable<IUnit> standardUnits)
    {
      DisplayName = "costs";
      _model = model;
      Currencies = standardUnits.Where(u => u.Dimension == "Currency")
                                .ToArray();
    }

    public IEnumerable<IUnit> Currencies { get; private set; }

    public IUnit Currency
    {
      get { return _model.Currency; }
      set
      {
        _model.Currency = value;
        NotifyOfPropertyChange(() => Currency);
      }
    }

    public double Price
    {
      get { return _model.Price; }
      set
      {
        _model.Price = value;
        NotifyOfPropertyChange(() => Price);
      }
    }
  }
}