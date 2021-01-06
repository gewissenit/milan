#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation.Observers;

namespace Milan.Simulation.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class OnDeleteEntityIsNotReferencedInAnyObserver : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public OnDeleteEntityIsNotReferencedInAnyObserver([Import] IJsonStore store)
    {
      _store = store;
    }

    [Import]
    private IDeleteManager DeleteManager { get; set; }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IEntity;
      if (target == null)
      {
        return;
      }
      foreach (var statistic in _store.Content.OfType<IModel>()
                                       .SelectMany(m => m.Observers)
                                       .OfType<IEntityObserver>()
                                       .Where(m => m.Entity == target).ToArray())
      {
        DeleteManager.Delete(statistic);
      }
    }
  }
}