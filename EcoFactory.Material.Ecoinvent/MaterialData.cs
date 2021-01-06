#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using FileHelpers;

namespace EcoFactory.Material.Ecoinvent
{
  [DelimitedRecord(";")]
// ReSharper disable ClassNeverInstantiated.Global
  public class MaterialData
// ReSharper restore ClassNeverInstantiated.Global
  {
//INFO: inconsitent naming required (class is a DTO for FileHelpers, naming is expected)
//INFO: name;location;category;subCategory;unit;type;ubp06;ipcc2007;metaData
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnassignedField.Global
    public string name;
    public string location;
    public string category;
    public string subCategory;
    public string unit;
    public MaterialType type;
    [FieldConverter(ConverterKind.Double, ",")]
    public double? ubp06;
    [FieldConverter(ConverterKind.Double, ",")]
    public double? ipcc2007;
    public string metaData;
// ReSharper restore UnassignedField.Global
// ReSharper restore FieldCanBeMadeReadOnly.Global
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore InconsistentNaming
    
    public string Category
    {
      get { return category; }
    }

    public double? Ipcc2007
    {
      get { return ipcc2007; }
    }

    public string MetaData
    {
      get { return metaData; }
    }

    public string Name
    {
      get { return name; }
    }

    public string Region
    {
      get { return location; }
    }

    public string SubCategory
    {
      get { return subCategory; }
    }

    public MaterialType Type
    {
      get { return type; }
    }

    public double? Ubp06
    {
      get { return ubp06; }
    }

    public string Unit
    {
      get { return unit; }
    }
  }
}