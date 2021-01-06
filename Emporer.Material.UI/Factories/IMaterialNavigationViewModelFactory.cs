#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Material;
using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.Factories
{
  public interface IMaterialNavigationViewModelFactory
  {
    IViewModel CreateNavigationViewModel(IMaterial model);
  }
}