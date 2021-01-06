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
  internal class MaterialIsNotReferencedInAnyContainedMaterial : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public MaterialIsNotReferencedInAnyContainedMaterial([Import] IJsonStore store)
    {
      _store = store;
    }

    [Import]
    private IDeleteManager DeleteManager { get; set; }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IMaterial;
      if (target == null)
      {
        return;
      }
      foreach (var containedMaterial in _store.Content.OfType<IMaterial>()
                                               .Where(m => m.Contains(target))
                                               .SelectMany(material => material.ContainedMaterials.ToArray()))
      {
        DeleteManager.Delete(containedMaterial);
      }
    }
  }
}