using Caliburn.Micro;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class FailureSectionViewModel : Screen
  {
    private readonly IWorkstationBase _model;

    public FailureSectionViewModel(IWorkstationBase model, IDistributionSelectionViewModelFactory distributionsViewModelFactory)
    {
      _model = model;
      DisplayName = "failure";

      FailureDuration =
        distributionsViewModelFactory.Create(new PropertyWrapper<IDistributionConfiguration>(() => _model.FailureDuration,
                                                                                             v => _model.FailureDuration = v));
      FailureOccurrence =
        distributionsViewModelFactory.Create(new PropertyWrapper<IDistributionConfiguration>(() => _model.FailureOccurrence,
                                                                                             v => _model.FailureOccurrence = v));
    }

    public DistributionSelectorViewModel FailureDuration { get; private set; }
    public DistributionSelectorViewModel FailureOccurrence { get; private set; }

    public bool CanFail
    {
      get { return _model.CanFail; }
      set
      {
        if (_model.CanFail == value)
        {
          return;
        }
        _model.CanFail = value;
        NotifyOfPropertyChange(() => CanFail);
      }
    }
  }
}