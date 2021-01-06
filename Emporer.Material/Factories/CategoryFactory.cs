#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;

namespace Emporer.Material.Factories
{
  [Export(typeof(ICategoryFactory))]
  internal class CategoryFactory : ICategoryFactory
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public CategoryFactory([Import] IJsonStore store)
    {
      _store = store;
    }

    public ICategory Create()
    {
      var category = new Category
                     {
                       Name = string.Empty
                     };

      _store.Add(category);
      return category;
    }

    public ICategory Duplicate(ICategory category)
    {
      var clone = new Category
                  {
                    Name = GetUniqueName(category.Name),
                    ParentCategory = category.ParentCategory
                  };

      _store.Add(clone);
      return clone;
    }

    private string GetUniqueName(string original)
    {
      var names = _store.Content.OfType<ICategory>()
                        .Select(e => e.Name);
      return Utils.GetUniqueName(original,
                                 names);
    }
  }
}