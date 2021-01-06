#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Emporer.Material;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.UI.ViewModels;

namespace Milan.UI.Factories
{
  [Export(typeof (ICategoryNavigationViewModelFactory))]
  internal class CategoryNavigationViewModelFactory : ICategoryNavigationViewModelFactory
  {
    private readonly IMaterialNavigationViewModelFactory _materialNavigationViewModelFactory;
    private readonly IDomainMessageBus _messageBus;
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public CategoryNavigationViewModelFactory([Import] IJsonStore store,
                                              [Import] IDomainMessageBus messageBus,
                                              [Import] IMaterialNavigationViewModelFactory materialNavigationViewModelFactory)
    {
      _store = store;
      _messageBus = messageBus;
      _materialNavigationViewModelFactory = materialNavigationViewModelFactory;
    }

    public IViewModel CreateNavigationViewModel(ICategory category)
    {
      return new CategoryNavigationViewModel(category, this, _materialNavigationViewModelFactory, _messageBus, _store);
    }
  }
}