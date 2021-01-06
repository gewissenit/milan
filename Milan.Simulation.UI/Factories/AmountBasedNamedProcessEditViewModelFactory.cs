using System.ComponentModel.Composition;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (INamedProcessViewModelFactory))]
  internal class AmountBasedNamedProcessEditViewModelFactory : INamedProcessViewModelFactory
  {
    private readonly IDistributionConfigurationViewModelFactory _distributionConfigurationViewModelFactory;

    [ImportingConstructor]
    public AmountBasedNamedProcessEditViewModelFactory([Import] IDistributionConfigurationViewModelFactory distributionConfigurationViewModelFactory)
    {
      _distributionConfigurationViewModelFactory = distributionConfigurationViewModelFactory;
    }

    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (AmountBasedNamedProcessConfiguration);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new AmountBasedNamedProcessEditViewModel(model as AmountBasedNamedProcessConfiguration, _distributionConfigurationViewModelFactory);
    }
  }
}