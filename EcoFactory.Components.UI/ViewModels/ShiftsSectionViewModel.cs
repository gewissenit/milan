using Caliburn.Micro;
using Milan.Simulation.ShiftSystems;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class ShiftsSectionViewModel : Screen
  {
    private readonly IWorkingTimeDependent _model;

    public ShiftsSectionViewModel(IWorkingTimeDependent model)
    {
      _model = model;
      DisplayName = "shifts";
    }

    public bool IsWorkingTimeDependent
    {
      get { return _model.IsWorkingTimeDependent; }
      set
      {
        if (_model.IsWorkingTimeDependent == value)
        {
          return;
        }
        _model.IsWorkingTimeDependent = value;
        NotifyOfPropertyChange(() => IsWorkingTimeDependent);
      }
    }
  }
}