#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using EcoFactory.Material.Ecoinvent.UI.ViewModels;
using Emporer.Material;
using Emporer.WPF;
using Emporer.WPF.ViewModels;
using Milan.UI.Factories;

namespace Milan.UI.ViewModels
{
  internal class MaterialNavigationViewModel : Screen, IViewModel
  {
    private readonly IContainedMaterialNavigationViewModelFactory _containedMaterialFactory;
    private readonly SortedObservableCollection<IViewModel> _containedMaterials;
    private readonly IMaterial _model;

    public MaterialNavigationViewModel(IMaterial model, IContainedMaterialNavigationViewModelFactory containedMaterialFactory)
    {
      _model = model;
      _containedMaterialFactory = containedMaterialFactory;
      _containedMaterials = new SortedObservableCollection<IViewModel>(new OrderCategoriesBeforeMaterialsThenByNameAlphabetically());
      
      model.ContainedMaterials.ForEach(cm => _containedMaterials.Add(_containedMaterialFactory.CreateNavigationViewModel(cm)));
      
      _model.PropertyChanged += UpdateRelatedLocalProperty;
      _model.ContainedMaterialAdded += AddContainedMaterial;
      _model.ContainedMaterialRemoved += RemoveContainedMaterial;
    }

    public ObservableCollection<IViewModel> ContainedMaterials
    {
      get { return _containedMaterials; }
    }

    public object Model
    {
      get { return _model; }
    }

    private void RemoveContainedMaterial(object sender, IContainedMaterial e)
    {
      var vm = _containedMaterials.SingleOrDefault(x => x.Model == e);

      if (vm == null)
      {
        return;
        //throw new InvalidOperationException("ContainedMaterials (VM) does not contain match for the given model.");
      }

      _containedMaterials.Remove(vm);
    }

    private void AddContainedMaterial(object sender, IContainedMaterial e)
    {
      var vm = _containedMaterialFactory.CreateNavigationViewModel(e);
      _containedMaterials.Add(vm);
    }

    private void UpdateRelatedLocalProperty(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          DisplayName = _model.Name;
          break;
      }
    }
  }
}