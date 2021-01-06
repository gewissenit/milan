using System.ComponentModel.Composition;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Resources.Factories;
using Milan.Simulation.Resources.UI.ViewModels;

namespace Milan.Simulation.Resources.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class ResourcePoolEditViewModelFactory : IEditViewModelFactory
  {
    private readonly IResourceTypeAmountFactory _resourceTypeAmountFactory;

    [ImportingConstructor]
    public ResourcePoolEditViewModelFactory([Import] IResourceTypeAmountFactory resourceTypeAmountFactory)
    {
      _resourceTypeAmountFactory = resourceTypeAmountFactory;
    }

    public bool CanHandle(object model)
    {
      return model is IResourcePool;
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new ResourcePoolEditViewModel((IResourcePool) model,
                                           new[]
                                           {
                                             new AvailableResourcesSectionViewModel((IResourcePool) model,
                                                                                    _resourceTypeAmountFactory),
                                           });
    }
  }
}