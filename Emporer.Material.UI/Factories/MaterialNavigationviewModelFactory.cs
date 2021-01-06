#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Emporer.Material.UI.ViewModels;
using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.Factories
{
  [Export(typeof (IMaterialNavigationViewModelFactory))]
  internal class MaterialNavigationViewModelFactory : IMaterialNavigationViewModelFactory
  {
    public IViewModel CreateNavigationViewModel(IMaterial model)
    {
      return new MaterialNavigationViewModel(model);
    }
  }
}