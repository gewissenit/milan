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
  internal class ContainedMaterialIsNotReferencedInAnyMaterial : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ContainedMaterialIsNotReferencedInAnyMaterial([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IContainedMaterial;
      if (target == null)
      {
        return;
      }
      foreach (var material in _store.Content.OfType<IMaterial>()
                                      .Where(m => m.ContainedMaterials.Contains(target)))
      {
        material.Remove(target);
      }
    }
  }
}