#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Emporer.Material.Factories;
using Emporer.Material.UI.ViewModels;
using Emporer.Unit.Factories;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;

namespace Emporer.Material.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class MaterialEditViewModelFactory : IEditViewModelFactory
  {
    private readonly IContainedMaterialFactory _containedMaterialFactory;
    private readonly IStandardUnitFactory _standardUnitFactory;
    private readonly IJsonStore _store;
    private readonly IUnitFactory _unitFactory;

    [ImportingConstructor]
    public MaterialEditViewModelFactory([Import] IUnitFactory unitFactory,
                                        [Import] IStandardUnitFactory standardUnitFactory,
                                        [Import] IJsonStore store,
                                        [Import] IContainedMaterialFactory containedMaterialFactory)
    {
      _unitFactory = unitFactory;
      _standardUnitFactory = standardUnitFactory;
      _store = store;
      _containedMaterialFactory = containedMaterialFactory;
    }

    public bool CanHandle(object model)
    {
      return model.GetType() == typeof(Material);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      var material = (IMaterial) model;
      return new MaterialEditViewModel(material,
                                       new Screen[]
                                       {
                                         new UnitSectionViewModel(material,
                                                                  _unitFactory,
                                                                  _standardUnitFactory.StandardUnits),
                                         new CostsSectionViewModel(material,
                                                                   _standardUnitFactory.StandardUnits),
                                         new ContainedMaterialSectionViewModel(material,
                                                                               _containedMaterialFactory,
                                                                               _store.Content.OfType<IMaterial>()),
                                         new CategoriesSectionViewModel(material,
                                                                        _store.Content.OfType<ICategory>())
                                       });
    }
  }
}