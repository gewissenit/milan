#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;

namespace Emporer.Material.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class CategoryIsNotReferencedInAnyCategoryAndCategorizable : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public CategoryIsNotReferencedInAnyCategoryAndCategorizable([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as ICategory;
      if (target == null)
      {
        return;
      }
      foreach (var child in _store.Content.OfType<ICategory>()
                                   .Where(c => c.ParentCategory == target))
      {
        child.ParentCategory = null;
      }
      foreach (var categorizable in _store.Content.OfType<ICategorizable>()
                                           .Where(c => c.Categories.Contains(target)))
      {
        categorizable.Remove(target);
      }
    }
  }
}