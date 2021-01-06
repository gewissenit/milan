#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using EcoFactory.Material.Ecoinvent.UI.ViewModels;
using Emporer.Material;
using Emporer.Material.Factories;
using Emporer.Material.UI.ViewModels;
using Emporer.WPF;
using Milan.JsonStore;
using Milan.Simulation.UI.ViewModels;
using Ork.Framework;

namespace Milan.UI.ViewModels
{
  [Export(typeof (IWorkspace))]
  public class MaterialPerspectiveViewModel : DocumentBase, IWorkspace
  {
    private readonly ICategoryFactory _categoryFactory;
    private readonly IDeleteManager _deleteManager;
    private readonly IMaterialFactory _materialFactory;
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public MaterialPerspectiveViewModel([Import] IJsonStore store,
                                        [Import] IPropertyEditorViewModel propertyEditorViewModel,
                                        [Import] EcoinventBrowserViewModel browserViewModel,
                                        [Import] IDeleteManager deleteManager,
                                        [Import] MaterialNavigatorViewModel materialNavigatorViewModel,
                                        [Import] IMaterialFactory materialFactory,
                                        [Import] ICategoryFactory categoryFactory,
                                        [Import] ISelection sharedSelection)
    {
      _deleteManager = deleteManager;
      Index = 20;
      DisplayName = "Materials";
      IsEnabled = true;
      EcoinventBrowser = browserViewModel;
      MaterialNavigator = materialNavigatorViewModel;
      _materialFactory = materialFactory;
      _categoryFactory = categoryFactory;
      PropertyEditor = propertyEditorViewModel;
      _store = store;

      MaterialNavigator.Selection = sharedSelection;
      PropertyEditor.Selection = sharedSelection;

      _store.ProjectChanged.Subscribe(_=> OnProjectChanged());
      MaterialNavigator.PropertyChanged += UpdateToSelectedItem;

      SetCurrentProject();
    }

    public EcoinventBrowserViewModel EcoinventBrowser { get; private set; }
    public MaterialNavigatorViewModel MaterialNavigator { get; private set; }
    public IPropertyEditorViewModel PropertyEditor { get; private set; }

    public bool CanAddContainedMaterial
    {
      get
      {
        var material = MaterialNavigator.SelectedItem as IMaterial;
        return material != null && material.ContainedMaterials.All(cm => cm.Material != null);
      }
    }

    public bool CanDuplicateSelected
    {
      get { return MaterialNavigator.SelectedItem is IMaterial || MaterialNavigator.SelectedItem is ICategory; }
    }

    public bool CanRemoveSelected
    {
      get { return MaterialNavigator.SelectedItem is IDomainEntity; }
    }

    public int Index { get; private set; }
    public bool IsEnabled { get; private set; }


    public void AddCategory()
    {
      var category = _categoryFactory.Create();
      //todo: has to be commented out because navigator does not update view model; should be using selection?
      //MaterialNavigator.SelectedItem = category;
    }

    public void AddMaterial()
    {
      var material = _materialFactory.Create();
      //todo: has to be commented out because navigator does not update view model; should be using selection?
      //MaterialNavigator.SelectedItem = material;
    }

    public void DuplicateSelected()
    {
      var material = MaterialNavigator.SelectedItem as IMaterial;
      if (material != null)
      {
        var clone = _materialFactory.Duplicate(material);
        //todo: has to be commented out because navigator does not update view model; should be using selection?
        //MaterialNavigator.SelectedItem = clone;
      }

      var category = MaterialNavigator.SelectedItem as ICategory;
      if (category != null)
      {
        var clone = _categoryFactory.Duplicate(category);
        //todo: has to be commented out because navigator does not update view model; should be using selection?
        //MaterialNavigator.SelectedItem = clone;
      }
    }

    public void RemoveSelected()
    {
      _deleteManager.Delete(MaterialNavigator.SelectedItem);
    }

    private void OnProjectChanged()
    {
      SetCurrentProject();
    }

    private void SetCurrentProject()
    {
      MaterialNavigator.Project = _store.Content;
    }

    private void UpdateToSelectedItem(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "SelectedItem")
      {
        NotifyOfPropertyChange(() => CanAddContainedMaterial);
        NotifyOfPropertyChange(() => CanDuplicateSelected);
        NotifyOfPropertyChange(() => CanRemoveSelected);
      }
    }

    public void HandleKeyInput(Key key)
    {
    }
  }
}