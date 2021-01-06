using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF.Commands;
using Milan.Simulation.Resources.Factories;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  /// <summary>
  ///   Edit the available resource of a <see cref="IResourcePool" />
  /// </summary>
  public sealed class AvailableResourcesSectionViewModel : Screen
  {
    private readonly IResourcePool _model;
    private readonly IResourceTypeAmountFactory _resourceTypeAmountFactory;

    public AvailableResourcesSectionViewModel(IResourcePool model, IResourceTypeAmountFactory resourceTypeAmountFactory)
    {
      DisplayName = "available resources";
      _model = model;
      _resourceTypeAmountFactory = resourceTypeAmountFactory;

      ResourceTypeAmounts = InitializeResourceTypeAmounts(_model.Resources);
      //todo: use factory
      AddResourceAmountCommand = new AddResourceTypeCommand(_model.Model, AddResource, _model.Resources);
      RemoveCommand = new RelayCommand(Remove);
    }

    public ICommand RemoveCommand { get; private set; }
    public ObservableCollection<ResourceTypeAmountEditorViewModel> ResourceTypeAmounts { get; private set; }
    public AddResourceTypeCommand AddResourceAmountCommand { get; private set; }

    public void Remove(object item)
    {
      var resourceVm = item as ResourceTypeAmountEditorViewModel;

      if (resourceVm == null)
      {
        return;
      }
      ResourceTypeAmounts.Remove(resourceVm);
      _model.Remove(resourceVm.Model);
      AddResourceAmountCommand.Refresh(_model.Resources);
      NotifyOfPropertyChange(() => AddResourceAmountCommand);
    }

    private ObservableCollection<ResourceTypeAmountEditorViewModel> InitializeResourceTypeAmounts(IEnumerable<IResourceTypeAmount> resourceTypeAmounts)
    {
      // create a vm for each RT related distribution
      //todo: use factory
      var vms = resourceTypeAmounts.Select(rta => new ResourceTypeAmountEditorViewModel(rta));
      return new ObservableCollection<ResourceTypeAmountEditorViewModel>(vms);
    }

    private void AddResource(IResourceType resourceType)
    {
      var resource = _resourceTypeAmountFactory.Create();
      resource.ResourceType = resourceType;
      resource.Amount = 1;
      _model.Add(resource);
      //todo: use factory
      ResourceTypeAmounts.Add(new ResourceTypeAmountEditorViewModel(resource));
      AddResourceAmountCommand.Refresh(_model.Resources);
    }
  }
}