using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  public class NamedProcessSelectorViewModel : Conductor<IEditViewModel>.Collection.OneActive
  {
    private readonly IList<IEditViewModel> _namedProcesses = new List<IEditViewModel>();
    private readonly PropertyWrapper<INamedProcessConfiguration> _propertyWrapper;

    public NamedProcessSelectorViewModel(IEnumerable<INamedProcessConfigurationFactory> factories,
                                         IEnumerable<IEditViewModelFactory> viewModelFactories,
                                         PropertyWrapper<INamedProcessConfiguration> propertyWrapper)
    {
      _propertyWrapper = propertyWrapper;
      
      
      var nullViewModel = new NullViewModel();
      _namedProcesses.Add(nullViewModel);
      
      var namedProcess = propertyWrapper.Value;
      foreach (var cfg in factories.Select(f => f.Create())
                          .Where(cfg => namedProcess == null || cfg.GetType() != namedProcess.GetType()))
      {
        _namedProcesses.Add(viewModelFactories.Single(factory => factory.CanHandle(cfg))
                                              .CreateEditViewModel(cfg));
      }
      
      if (namedProcess != null)
      {
        var viewModel = viewModelFactories.Single(factory => factory.CanHandle(namedProcess))
                                          .CreateEditViewModel(namedProcess);
        _namedProcesses.Add(viewModel);
        SelectedProcess = viewModel;
      }
      else
      {
        SelectedProcess = nullViewModel;
      }
    }

    public IEnumerable<IEditViewModel> NamedProcesses
    {
      get { return _namedProcesses; }
    }

    private IEditViewModel _selectedProcess;

    public IEditViewModel SelectedProcess
    {
      get { return _selectedProcess; }
      set
      {
        if (_selectedProcess == value)
        {
          return;
        }
        _selectedProcess = value;
        _propertyWrapper.Value = (INamedProcessConfiguration) value.Model;
        NotifyOfPropertyChange(() => SelectedProcess);
      }
    }
  }
}