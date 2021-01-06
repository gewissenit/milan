#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.Factories
{
  public interface ICategoryNavigationViewModelFactory
  {
    IViewModel CreateNavigationViewModel(ICategory category);
  }
}