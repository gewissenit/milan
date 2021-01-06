#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace EcoFactory.Material.Ecoinvent.Factories
{
  public interface IEcoinventMaterialFactory
  {
    void ImportMaterials(IEnumerable<MaterialData> ecoinventMaterials);
    IEnumerable<MaterialData> EcoInventMaterials { get; }
  }
}