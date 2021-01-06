#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  internal class CategoryNavigationViewModel : Screen, IViewModel
  {
    private readonly ICategoryNavigationViewModelFactory _categoryNavigationViewModelFactory;
    private readonly SortedObservableCollection<IViewModel> _children;
    private readonly IMaterialNavigationViewModelFactory _materialNavigationViewModelFactory;
    private readonly ICategory _model;
    private readonly IJsonStore _store;

    public CategoryNavigationViewModel(ICategory model,
                                       ICategoryNavigationViewModelFactory categoryNavigationViewModelFactory,
                                       IMaterialNavigationViewModelFactory materialNavigationViewModelFactory,
                                       IDomainMessageBus messageBus,
                                       IJsonStore store)
    {
      _children = new SortedObservableCollection<IViewModel>(new OrderCategoriesBeforeMaterialsThenByNameAlphabetically(), "DisplayName");

      _model = model;

      _categoryNavigationViewModelFactory = categoryNavigationViewModelFactory;
      _materialNavigationViewModelFactory = materialNavigationViewModelFactory;
      _store = store;

      UpdateAllChildren();

      var categoryAdded = messageBus.EntityAdded.OfType<ICategory>();

      categoryAdded.Where(c => c.ParentCategory == _model)
                   .Subscribe(Add);
      categoryAdded.Subscribe(AddListener);

      messageBus.EntityRemoved.OfType<ICategory>()
                  .Where(c => _children.Any(x => x.Model == c))
                  .Subscribe(Remove);

      var materialAdded = messageBus.EntityAdded.OfType<IMaterial>();
      materialAdded.Where(m => m.Categories.Contains(_model))
                   .Subscribe(Add);
      materialAdded.Subscribe(AddListener);

      messageBus.EntityRemoved.OfType<IMaterial>()
                  .Where(m => _children.Any(x => x.Model == m))
                  .Where(m => m.Categories.Contains(_model))
                  .Subscribe(Remove);
      
      _model.PropertyChanged += UpdateLocalProperties;
      DisplayName = _model.Name;
    }

    public IEnumerable<IViewModel> Children
    {
      get { return _children; }
    }

    public object Model
    {
      get { return _model; }
    }

    private void Add<T>(T entity)
    {
      var viewModel = CreateByMatchingFactory(entity);
      _children.Add(viewModel);
    }

    private IViewModel CreateByMatchingFactory<T>(T entity)
    {
      if (entity is ICategory)
      {
        return _categoryNavigationViewModelFactory.CreateNavigationViewModel((ICategory) entity);
      }

      if (entity is IMaterial)
      {
        return _materialNavigationViewModelFactory.CreateNavigationViewModel((IMaterial) entity);
      }
      throw new InvalidOperationException();
    }

    private void Remove<T>(T entity)
    {
      var vm = Children.Single(x => x.Model == (object) entity);

      _children.Remove(vm);
    }

    private void UpdateAllChildren()
    {
      var categories = _store.Content.OfType<ICategory>()
                              .ToArray();
      var childCategories = categories.Where(x => x.ParentCategory == _model)
                                      .Select(_categoryNavigationViewModelFactory.CreateNavigationViewModel);
      var materials = _store.Content.OfType<IMaterial>()
                             .ToArray();
      var containedMaterials = materials.Where(x => x.Categories.Contains(_model))
                                        .Select(_materialNavigationViewModelFactory.CreateNavigationViewModel);
      materials.ForEach(AddListener);
      categories.Except(new[]
                        {
                          _model
                        })
                .ForEach(AddListener);

      childCategories.Concat(containedMaterials)
                     .ForEach(_children.Add);
    }

    private void AddListener(ICategory c)
    {
      c.PropertyChanged += (s, e) =>
                           {
                             if (e.PropertyName != "ParentCategory")
                             {
                               return;
                             }
                             if (c.ParentCategory == _model)
                             {
                               Add(c);
                             }
                             if (c.ParentCategory != _model &&
                                 _children.Any(ch => ch.Model == c))
                             {
                               Remove(c);
                             }
                           };
    }

    private void AddListener(IMaterial m)
    {
      m.Added += (s, e) =>
                 {
                   if (e == _model)
                   {
                     Add(m);
                   }
                 };
      m.Removed += (s, e) =>
                   {
                     if (e == _model)
                     {
                       Remove(m);
                     }
                   };
    }

    private void UpdateLocalProperties(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          DisplayName = _model.Name;
          NotifyOfPropertyChange(() => DisplayName);
          break;
      }
    }
  }
}