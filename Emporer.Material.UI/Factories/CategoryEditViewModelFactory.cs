#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Emporer.Material.UI.ViewModels;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;

namespace Emporer.Material.UI.Factories
{
  [Export(typeof (IEditViewModelFactory))]
  internal class CategoryEditViewModelFactory : IEditViewModelFactory
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public CategoryEditViewModelFactory([Import] IJsonStore store)
    {
      _store = store;
    }

    public bool CanHandle(object model)
    {
      return model.GetType() == typeof (Category);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new CategoryEditViewModel(model as ICategory, _store.Content.OfType<ICategory>());
    }
  }
}