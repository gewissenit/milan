#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Emporer.Material;
using Emporer.WPF.ViewModels;
using Milan.UI.ViewModels;

namespace Milan.UI.Factories
{
  [Export(typeof (IMaterialNavigationViewModelFactory))]
  internal class MaterialNavigationViewModelFactory : IMaterialNavigationViewModelFactory
  {
    private readonly IContainedMaterialNavigationViewModelFactory _containedMaterialNavigationViewModelFactory;

    [ImportingConstructor]
    public MaterialNavigationViewModelFactory([Import] IContainedMaterialNavigationViewModelFactory containedMaterialNavigationViewModelFactory)
    {
      _containedMaterialNavigationViewModelFactory = containedMaterialNavigationViewModelFactory;
    }

    public IViewModel CreateNavigationViewModel(IMaterial model)
    {
      return new MaterialNavigationViewModel(model, _containedMaterialNavigationViewModelFactory);
    }
  }
}