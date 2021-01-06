using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  internal abstract class NamedProcessEditViewModel : EditViewModel
  {
    protected NamedProcessEditViewModel(NamedProcessConfiguration model,
                                        IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
      : base(model, model.Name)
    {
      DistributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
      Duration = new DistributionSelectorViewModel(distributionConfigurationViewModelFactory,
                                                   new PropertyWrapper<IDistributionConfiguration>(() => model.Duration, v => model.Duration = v));
    }

    protected IDistributionConfigurationViewModelFactory DistributionConfigurationViewModelFactory { get; private set; }

    public DistributionSelectorViewModel Duration { get; private set; }
  }
}