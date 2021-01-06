using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.Material.Factories;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.ViewModels
{
  public class ContainedMaterialSectionViewModel : Screen
  {
    private const string Param = "contained material";
    private readonly IMaterial _model;
    private readonly IContainedMaterialFactory _containedMaterialFactory;
    private readonly ObservableCollection<object> _availableMaterials = new ObservableCollection<object>();
    private readonly IEnumerable<IMaterial> _materials;

    public ContainedMaterialSectionViewModel(IMaterial model,
                                             IContainedMaterialFactory containedMaterialFactory,
                                             IEnumerable<IMaterial> materials)
    {
      DisplayName = "content";
      _model = model;
      _containedMaterialFactory = containedMaterialFactory;
      _materials = materials;
      ContainedMaterials = WrapContainedMaterials(_model.ContainedMaterials);
      UpdateAvailableMaterials();
      var productTypeParameterSet = new ParameterSet(Param,
                                                     "Select a containing material",
                                                     _availableMaterials);

      AddContainedMaterialCommand = new ChainedParameterCommandViewModel(new[]
                                                                         {
                                                                           productTypeParameterSet
                                                                         },
                                                                         AddContainedMaterial)
                                    {
                                      DisplayText = "Add"
                                    };
      RemoveCommand = new RelayCommand(Remove);
    }

    public ObservableCollection<ContainedMaterialEditorViewModel> ContainedMaterials { get; private set; }
    public ChainedParameterCommandViewModel AddContainedMaterialCommand { get; private set; }
    public ICommand RemoveCommand { get; private set; }

    private void UpdateAvailableMaterials()
    {
      _availableMaterials.Clear();
      var containedMaterials = _model.ContainedMaterials.Select(cm => cm.Material);
      var allParents = GetAllParents(_materials,
                                     _model)
        .Distinct();
      foreach (var material in _materials.Except(containedMaterials)
                                         .Except(new[]
                                                 {
                                                   _model
                                                 })
                                         .Except(allParents))
      {
        _availableMaterials.Add(material);
      }
    }


    public void Remove(object item)
    {
      if (item is ContainedMaterialEditorViewModel)
      {
        RemoveContainedMaterial((ContainedMaterialEditorViewModel) item);
      }
    }

    private void AddContainedMaterial(IDictionary<string, object> parameters)
    {
      var destination = (IMaterial) parameters[Param];

      var containedMaterial = _containedMaterialFactory.Create();
      containedMaterial.Material = destination;
      _model.Add(containedMaterial);

      var vm = new ContainedMaterialEditorViewModel(containedMaterial,
                                                    _model);
      ContainedMaterials.Add(vm);
      UpdateAvailableMaterials();
    }

    private void RemoveContainedMaterial(ContainedMaterialEditorViewModel item)
    {
      ContainedMaterials.Remove(item);
      var model = (IContainedMaterial) item.Model;
      _model.Remove(model);
      UpdateAvailableMaterials();
    }

    private ObservableCollection<ContainedMaterialEditorViewModel> WrapContainedMaterials(IEnumerable<IContainedMaterial> containedMaterials)
    {
      var vms = containedMaterials.Select(con => new ContainedMaterialEditorViewModel(con,
                                                                                      _model));
      return new ObservableCollection<ContainedMaterialEditorViewModel>(vms);
    }

    private static IEnumerable<IMaterial> GetAllParents(IEnumerable<IMaterial> materials,
                                                        IMaterial material)
    {
      foreach (var candidate in materials.Where(candidate => candidate.Contains(material)))
      {
        yield return candidate;

        foreach (var candidateParent in GetAllParents(materials,
                                                      candidate))
        {
          yield return candidateParent;
        }
      }
    }
  }
}