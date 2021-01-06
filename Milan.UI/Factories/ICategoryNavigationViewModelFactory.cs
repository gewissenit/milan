#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Material;
using Emporer.WPF.ViewModels;

namespace Milan.UI.Factories
{
  public interface ICategoryNavigationViewModelFactory
  {
    IViewModel CreateNavigationViewModel(ICategory category);
  }
}