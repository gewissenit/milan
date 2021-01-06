using System.ComponentModel.Composition;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (IExperimentNavigatorModelViewModelFactory))]
  internal class ExperimentNavigatorModelViewModelFactory : IExperimentNavigatorModelViewModelFactory
  {
    private readonly ISimulationService _simulationService;

    [ImportingConstructor]
    public ExperimentNavigatorModelViewModelFactory([Import] ISimulationService simulationService)
    {
      _simulationService = simulationService;
    }

    public ExperimentNavigationViewModel CreateNavigationViewModel(IModel model)
    {
      return new ExperimentNavigationViewModel(model, _simulationService);
    }
  }
}