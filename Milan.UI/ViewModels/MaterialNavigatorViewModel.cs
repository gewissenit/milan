#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using EcoFactory.Material.Ecoinvent.UI.ViewModels;
using Emporer.Material;
using Emporer.WPF;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.UI.Factories;

namespace Milan.UI.ViewModels
{
  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class MaterialNavigatorViewModel : PropertyChangedBase
  {
    private readonly ICategoryNavigationViewModelFactory _categoryNavigationViewModelFactory;
    private readonly IMaterialNavigationViewModelFactory _materialNavigationViewModelFactory;
    private IEnumerable<object> _project;
    private object _selectedItem;
    private IViewModel _selectedViewModelItem;
    private ISelection _selection;

    [ImportingConstructor]
    public MaterialNavigatorViewModel([Import] IDomainMessageBus messageBus,
                                      [Import] IMaterialNavigationViewModelFactory materialNavigationViewModelFactory,
                                      [Import] ICategoryNavigationViewModelFactory categoryNavigationViewModelFactory)
    {
      _materialNavigationViewModelFactory = materialNavigationViewModelFactory;
      _categoryNavigationViewModelFactory = categoryNavigationViewModelFactory;

      RootItems = new SortedObservableCollection<IViewModel>(new OrderCategoriesBeforeMaterialsThenByNameAlphabetically(), "DisplayName");

      var categoryAdded = messageBus.EntityAdded.OfType<ICategory>();
      categoryAdded.Where(IsRootCategory)
                   .Subscribe(AddRoot);
      categoryAdded.Subscribe(AddListener);

      var materialAdded = messageBus.EntityAdded.OfType<IMaterial>();
      materialAdded.Where(IsRootMaterial)
                   .Subscribe(AddRoot);
      materialAdded.Subscribe(AddListener);
      
      messageBus.EntityRemoved.OfType<ICategory>()
                  .Where(IsRootCategory)
                  .Subscribe(RemoveRoot);

      messageBus.EntityRemoved.OfType<IMaterial>()
                  .Where(IsRootMaterial)
                  .Subscribe(RemoveRoot);
    }


    public IEnumerable<object> Project
    {
      get { return _project; }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException();
        }

        if (_project == value)
        {
          return;
        }

        _project = value;
        UpdateToProject();
        NotifyOfPropertyChange(() => Project);
      }
    }

    public SortedObservableCollection<IViewModel> RootItems { get; private set; }

    public object SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem == value)
        {
          return;
        }
        _selectedItem = value;
        NotifyOfPropertyChange(() => SelectedItem);
        _selection.Select(_selectedItem, this);
      }
    }


    public IViewModel SelectedViewModelItem
    {
      get { return _selectedViewModelItem; }
      set
      {
        if (_selectedViewModelItem == value)
        {
          return;
        }
        _selectedViewModelItem = value;
        NotifyOfPropertyChange(() => SelectedViewModelItem);
        if (_selectedViewModelItem == null)
        {
          return;
        }
        SelectedItem = _selectedViewModelItem.Model;
      }
    }

    public ISelection Selection 
    {
      get { return _selection; }
      set
      {
        _selection = value;
        _selection.Subscribe<object>(this, Select);
      }
    }

    private void Select(object item)
    {
      SelectedItem = item;
    }

    private static bool IsRootMaterial(IMaterial material)
    {
      return !material.Categories.Any();
    }

    private static bool IsRootCategory(ICategory category)
    {
      return category.ParentCategory == null;
    }

    private void RemoveRoot(object model)
    {
      var root = RootItems.Single(x => x.Model == model);
      RootItems.Remove(root);
    }

    private void AddRoot(ICategory model)
    {
      RootItems.Add(_categoryNavigationViewModelFactory.CreateNavigationViewModel(model));
    }

    private void AddRoot(IMaterial model)
    {
      RootItems.Add(_materialNavigationViewModelFactory.CreateNavigationViewModel(model));
    }

    private void UpdateToProject()
    {
      RootItems.Clear();
      var categories = Project.OfType<ICategory>()
                              .ToArray();
      var rootCategories = categories.Where(x => x.ParentCategory == null)
                                     .Select(_categoryNavigationViewModelFactory.CreateNavigationViewModel);
      categories.ForEach(AddListener);
      var materials = Project.OfType<IMaterial>()
                             .ToArray();
      var rootMaterials = materials.Where(x => !x.Categories.Any())
                                   .Select(_materialNavigationViewModelFactory.CreateNavigationViewModel);
      materials.ForEach(AddListener);
      rootCategories.Concat(rootMaterials)
                    .ForEach(x => RootItems.Add(x));
    }

    private void AddListener(ICategory c)
    {
      c.PropertyChanged += (s, e) =>
                           {
                             if (e.PropertyName != "ParentCategory")
                             {
                               return;
                             }
                             if (IsRootCategory(c) && RootItems.All(ri => ri.Model != c))
                             {
                               AddRoot(c);
                             }
                             else if (!IsRootCategory(c) && RootItems.Any(ri => ri.Model == c))
                             {
                               RemoveRoot(c);
                             }
                           };
    }

    private void AddListener(IMaterial m)
    {
      m.Added += (s, e) =>
                 {
                   if (!IsRootMaterial(m) && RootItems.Any(ri=>ri.Model == m))
                   {
                     RemoveRoot(m);
                   }
                 };
      m.Removed += (s, e) =>
                   {
                     if (IsRootMaterial(m) && RootItems.All(ri => ri.Model != m))
                     {
                       AddRoot(m);
                     }
                   };
    }
  }
}