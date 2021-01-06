#region License

// Copyright (c) 2013 HTW Berlin All rights reserved.

#endregion License

using Emporer.Unit;
using Milan.JsonStore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using GeWISSEN.Utils;

namespace Emporer.Material
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Material : DomainEntity, IMaterial
  {
    private readonly IList<ICategory> _categories = new List<ICategory>();

    private readonly IList<IMaterialProperty> _properties = new List<IMaterialProperty>();

    private readonly IList<IContainedMaterial> _containedMaterials = new List<IContainedMaterial>();

    [JsonProperty]
    public IUnit Currency
    {
      get { return Get<IUnit>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Description
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IUnit DisplayUnit
    {
      get { return Get<IUnit>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Name
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IUnit OwnUnit
    {
      get { return Get<IUnit>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Price
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IEnumerable<ICategory> Categories
    {
      get { return _categories; }
    }

    [JsonProperty]
    public IEnumerable<IMaterialProperty> Properties
    {
      get { return _properties; }
    }

    [JsonProperty]
    public IEnumerable<IContainedMaterial> ContainedMaterials
    {
      get { return _containedMaterials; }
    }

    public event Action<ICategorizable, ICategory> Added;

    public event Action<ICategorizable, ICategory> Removed;

    public event Action<IContainedMaterial> ContainedMaterialAdded;

    public event Action<IContainedMaterial> ContainedMaterialRemoved;

    public void Add(ICategory category)
    {
      Throw.IfNull(category, "category");
      _categories.Add(category);
      Added.Raise(this, category);
    }

    public void Remove(ICategory category)
    {
      Throw.IfNull(category, "category");
      _categories.Remove(category);
      Removed.Raise(this, category);
    }

    public void Add(IMaterialProperty property)
    {
      _properties.Add(property);
    }

    public void Remove(IMaterialProperty property)
    {
      _properties.Remove(property);
    }

    public void Add(IContainedMaterial containedMaterial)
    {
      if (_containedMaterials.Any(cm => cm.Material == containedMaterial.Material))
      {
        throw new ArgumentException();
      }
      _containedMaterials.Add(containedMaterial);
      ContainedMaterialAdded.Raise(containedMaterial);
    }
    public void Remove(IContainedMaterial containedMaterial)
    {
      _containedMaterials.Remove(containedMaterial);
      ContainedMaterialRemoved.Raise(containedMaterial);
    }

    public bool Contains(IMaterial containedMaterial)
    {
      var entry = _containedMaterials.FirstOrDefault(cm => cm.Material == containedMaterial);
      return entry != null;
    }

    public override string ToString()
    {
      return string.Format("{0} (in {1})", Name, DisplayUnit);
    }
  }
}