#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Material;
using Milan.JsonStore;

namespace Milan.Simulation.MaterialAccounting.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class MaterialIsNotReferencedInAnyObserver : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public MaterialIsNotReferencedInAnyObserver([Import] IJsonStore store)
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
      foreach (var statistic in _store.Content.OfType<IModel>()
                                       .SelectMany(m => m.Observers)
                                       .OfType<IMaterialObserver>()
                                       .Where(m => m.Material == target).ToArray())
      {
        DeleteManager.Delete(statistic);
      }
    }
  }
}