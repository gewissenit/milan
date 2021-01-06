using Emporer.Math.Distribution;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.Math.Distribution.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  internal class TimeBasedNamedProcessEditViewModel : NamedProcessEditViewModel
  {
    public TimeBasedNamedProcessEditViewModel(TimeBasedNamedProcessConfiguration model,
                                              IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
      : base(model, distributionConfigurationViewModelFactory)
    {
      Occurrence = new DistributionSelectorViewModel(distributionConfigurationViewModelFactory,
                                                     new PropertyWrapper<IDistributionConfiguration>(() => model.Occurrence, v => model.Occurrence = v));
    }

    public DistributionSelectorViewModel Occurrence { get; private set; }
  }
}