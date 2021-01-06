#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;
using Newtonsoft.Json;

namespace Emporer.Material
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Category : DomainEntity, ICategory
  {
    [JsonProperty]
    public string Name
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Description
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public ICategory ParentCategory
    {
      get { return Get<ICategory>(); }
      set
      {
        Set(value);
        if (value != null)
        {
          SetParentToNull(value);
        }
      }
    }

    private void SetParentToNull(ICategory category)
    {
      if (category.ParentCategory == this)
      {
        category.ParentCategory = null;
      }
      else if (category.ParentCategory != null)
      {
        SetParentToNull(category.ParentCategory);
      }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}