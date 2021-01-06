#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Emporer.Unit;

namespace Emporer.Material
{
  public interface IMaterial : ICategorizable
  {
    IEnumerable<IContainedMaterial> ContainedMaterials { get; }
    IEnumerable<IMaterialProperty> Properties { get; }
    string Name { get; set; }
    string Description { get; set; }
    IUnit OwnUnit { get; set; }
    IUnit DisplayUnit { get; set; }
    IUnit Currency { get; set; }
    double Price { get; set; }
    void Add(IContainedMaterial containedMaterial);
    void Remove(IContainedMaterial containedMaterial);
    bool Contains(IMaterial containedMaterial);
    void Add(IMaterialProperty property);
    void Remove(IMaterialProperty property);
    event Action<IContainedMaterial> ContainedMaterialAdded;
    event Action<IContainedMaterial> ContainedMaterialRemoved;
  }
}