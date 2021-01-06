// Copyright (c) 2013 HTW Berlin All rights reserved.

using Caliburn.Micro;
using Emporer.WPF.Commands;
using Milan.Simulation.Factories;
using System.Windows.Input;

namespace Milan.Simulation.UI.ViewModels
{
  public class EntityFactoryViewModel : PropertyChangedBase
  {
    private readonly IEntityFactory _factory;
    private readonly IModel _model;
    private int _number;

    public EntityFactoryViewModel(IEntityFactory factory, IModel model)
    {
      _factory = factory;
      _model = model;
    }

    public string Name
    {
      get { return _factory.Name; }
    }

    public int Number
    {
      get
      {
        return _number;
      }
      set
      {
        if (_number == value)
        {
          return;
        }
        if (value < 0)
        {
          return;
        }
        _number = value;
        NotifyOfPropertyChange(() => Number);
      }
    }

    private RelayCommand _IncreaseAmountCommand;

    public ICommand IncreaseAmountCommand
    {
      get
      {
        if (_IncreaseAmountCommand == null)
        {
          _IncreaseAmountCommand = new RelayCommand(a => IncreaseAmount());
        }
        return _IncreaseAmountCommand;
      }
    }

    private void IncreaseAmount()
    {
      Number++;
    }

    private RelayCommand _DecreaseAmountCommand;

    public ICommand DecreaseAmountCommand
    {
      get
      {
        if (_DecreaseAmountCommand == null)
        {
          _DecreaseAmountCommand = new RelayCommand(a => DecreaseAmount());
        }
        return _DecreaseAmountCommand;
      }
    }

    private void DecreaseAmount()
    {
      Number++;
    }

    public void Create()
    {
      _factory.Create(_model);
    }
  }
}