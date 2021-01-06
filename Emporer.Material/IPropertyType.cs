#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;

namespace Emporer.Material
{
  public interface IPropertyType : IDomainEntity
  {
    string Name { get; set; }
    string Location { get; set; }
    string EcoCat { get; set; }
    string EcoSubCat { get; set; }
    string Unit { get; set; }
    string DataSourceId { get; set; }
  }
}