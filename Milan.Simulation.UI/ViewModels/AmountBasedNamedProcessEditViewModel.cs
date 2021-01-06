using Emporer.Math.Distribution.UI.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  internal class AmountBasedNamedProcessEditViewModel : NamedProcessEditViewModel
  {
    private readonly AmountBasedNamedProcessConfiguration _model;

    public AmountBasedNamedProcessEditViewModel(AmountBasedNamedProcessConfiguration model,
                                                IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
      : base(model, distributionConfigurationViewModelFactory)
    {
      _model = model;
    }

    public bool PerBatch
    {
      get { return _model.PerBatch; }
      set
      {
        _model.PerBatch = value;
        NotifyOfPropertyChange(() => PerBatch);
      }
    }

    public int Amount
    {
      get { return _model.Amount; }
      set
      {
        _model.Amount = value;
        NotifyOfPropertyChange(() => Amount);
      }
    }
  }
}