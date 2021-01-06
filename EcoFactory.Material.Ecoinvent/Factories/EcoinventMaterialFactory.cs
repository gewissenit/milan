#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Emporer.Material;
using Emporer.Material.Factories;
using Emporer.Unit;
using Emporer.Unit.Factories;
using Milan.JsonStore;

namespace EcoFactory.Material.Ecoinvent.Factories
{
  [Export(typeof (IEcoinventMaterialFactory))]
  internal class EcoinventMaterialFactory : IEcoinventMaterialFactory
  {
    private const string Ubp06 = "UBP06";
    private const string Ipcc2007 = "IPCC2007";
    private const string DataFile = @"EcoFactory.Material.Ecoinvent.Resources.EcoinventData.csv";
    private static IEnumerable<MaterialData> _ecoInventMaterials;
    private readonly ICategoryFactory _categoryFactory;
    private readonly IMaterialFactory _materialFactory;
    private readonly IMaterialPropertyFactory _materialPropertyFactory;
    private readonly IPropertyTypeFactory _propertyTypeFactory;
    private readonly IJsonStore _store;
    private readonly IUnitFactory _unitFactory;

    [ImportingConstructor]
    public EcoinventMaterialFactory([Import] IJsonStore store,
                                    [Import] ICategoryFactory categoryFactory,
                                    [Import] IUnitFactory unitFactory,
                                    [Import] IMaterialFactory materialFactory,
                                    [Import] IPropertyTypeFactory propertyTypeFactory,
                                    [Import] IMaterialPropertyFactory materialPropertyFactory)
    {
      _store = store;
      _categoryFactory = categoryFactory;
      _unitFactory = unitFactory;
      _materialFactory = materialFactory;
      _propertyTypeFactory = propertyTypeFactory;
      _materialPropertyFactory = materialPropertyFactory;
    }

    public IEnumerable<MaterialData> EcoInventMaterials
    {
      get
      {
        return _ecoInventMaterials = _ecoInventMaterials ?? (_ecoInventMaterials = LoadMaterialsFromCsv());
      }
    }

    public void ImportMaterials(IEnumerable<MaterialData> ecoinventMaterials)
    {
      // TODO: check existence before import?
      // assert the existence of the impact factor material property types
      // this is done a little complicated (out, array) to avoid too many calls to store.GetItems<>
      IPropertyType[] propertyTypes;
      EnsureExistenceOfImpactFactorMaterialProperties(out propertyTypes);
      var ubp06 = propertyTypes[0];
      var ipcc2007 = propertyTypes[1];

      // transform material dto to material domain entity
      foreach (var importMaterial in ecoinventMaterials)
      {

        var unit = _store.Content.OfType<IUnit>()
                         .SingleOrDefault(x => x.Symbol == importMaterial.Unit);

        if (unit == null)
        {
          unit = _unitFactory.Create();
          unit.Name = importMaterial.Unit;
          unit.Symbol = importMaterial.Unit;
        }

        var material = _materialFactory.Create();

        material.Name = importMaterial.Name;
        material.Description = importMaterial.MetaData;
        material.DisplayUnit = unit;
        material.OwnUnit = unit;
        material.IsReadonly = true;

        AddEcoinventCategories(material, importMaterial.Category, importMaterial.SubCategory);

        if (importMaterial.Ubp06.HasValue)
        {
          AddImpactFactorValueAsMaterialProperty(material, ubp06, importMaterial.Ubp06.Value);
        }

        if (importMaterial.Ipcc2007.HasValue)
        {
          AddImpactFactorValueAsMaterialProperty(material, ipcc2007, importMaterial.Ipcc2007.Value);
        }
      }
    }

    private IEnumerable<MaterialData> LoadMaterialsFromCsv()
    {
      IEnumerable<MaterialData> materials;
      using (var stream = Assembly.GetExecutingAssembly()
                                  .GetManifestResourceStream(DataFile))
      {
        materials = Utils.GetRecordsFromStream<MaterialData>(stream, 1, Encoding.UTF8);
      }

      return materials;
    }

    private void EnsureExistenceOfImpactFactorMaterialProperties(out IPropertyType[] propertyTypes)
    {
      var ubp06 = AddPropertyType(Ubp06, "UBP");
      var ipcc2007 = AddPropertyType(Ipcc2007, "kg");

      propertyTypes = new[]
                      {
                        ubp06, ipcc2007
                      };
    }

    private IPropertyType AddPropertyType(string name, string unit)
    {
      var propertyTypes = _store.Content.OfType<IPropertyType>();

      var propertyType = propertyTypes.SingleOrDefault(x => x.Name == name && x.DataSourceId == name);

      if (propertyType == null)
      {
        propertyType = _propertyTypeFactory.Create();

        propertyType.Name = name;
        propertyType.DataSourceId = name;
        propertyType.Location = string.Empty;
        propertyType.Unit = unit;
        propertyType.EcoCat = string.Empty;
        propertyType.EcoSubCat = string.Empty;
        _store.Add(propertyType);
      }
      return propertyType;
    }

    private void AddEcoinventCategories(IMaterial material, string categoryName, string subCategoryName)
    {
      var category = _store.Content.OfType<ICategory>()
                           .FirstOrDefault(x => x.Name == categoryName);

      if (category == null)
      {
        category = _categoryFactory.Create();
        category.Name = categoryName;
        category.IsReadonly = true;
      }

      var subCategory = _store.Content.OfType<ICategory>()
                              .FirstOrDefault(x => x.Name == subCategoryName && x.ParentCategory != null && x.ParentCategory.Name == categoryName);
      if (subCategory == null)
      {
        subCategory = _categoryFactory.Create();
        subCategory.Name = subCategoryName;
        subCategory.IsReadonly = true;
        subCategory.ParentCategory = category;
      }

      material.Add(subCategory);
    }

    private void AddImpactFactorValueAsMaterialProperty(IMaterial material, IPropertyType type, double amount)
    {
      var property = _materialPropertyFactory.Create();
      property.PropertyType = type;
      property.Mean = amount;
      material.Add(property);
    }
  }
}