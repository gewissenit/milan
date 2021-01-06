#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Unit.Factories;
using Milan.JsonStore;

namespace Emporer.Material.Factories
{
  [Export(typeof(IMaterialFactory))]
  internal class MaterialFactory : IMaterialFactory
  {
    private readonly IContainedMaterialFactory _containedMaterialFactory;
    private readonly IMaterialPropertyFactory _materialPropertyFactory;
    private readonly IJsonStore _store;
    private readonly IUnitFactory _unitFactory;

    [ImportingConstructor]
    public MaterialFactory([Import] IJsonStore store,
                           [Import] IUnitFactory unitFactory,
                           [Import] IMaterialPropertyFactory materialPropertyFactory,
                           [Import] IContainedMaterialFactory containedMaterialFactory)
    {
      _store = store;
      _unitFactory = unitFactory;
      _materialPropertyFactory = materialPropertyFactory;
      _containedMaterialFactory = containedMaterialFactory;
    }

    public IMaterial Create()
    {
      var material = new Material
                     {
                       Name = string.Empty
                     };
      _store.Add(material);
      return material;
    }

    public IMaterial Duplicate(IMaterial material)
    {
      var clone = new Material
                  {
                    Name = GetUniqueName(material.Name),
                    Description = material.Description,
                    Price = material.Price,
                    Currency = material.Currency,
                  };
      if (material.OwnUnit != null)
      {
        clone.OwnUnit = _unitFactory.Duplicate(material.OwnUnit);
      }

      clone.DisplayUnit = material.DisplayUnit == material.OwnUnit
                            ? clone.OwnUnit
                            : material.DisplayUnit;

      foreach (var property in material.Properties)
      {
        clone.Add(_materialPropertyFactory.Duplicate(property));
      }

      foreach (var category in material.Categories)
      {
        clone.Add(category);
      }

      foreach (var containedMaterial in clone.ContainedMaterials)
      {
        clone.Add(_containedMaterialFactory.Duplicate(containedMaterial));
      }
      _store.Add(clone);
      return clone;
    }

    private string GetUniqueName(string original)
    {
      var names = _store.Content.OfType<IMaterial>()
                        .Select(e => e.Name);
      return Utils.GetUniqueName(original,
                                 names);
    }
  }
}