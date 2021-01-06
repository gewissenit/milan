#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Milan.JsonStore;
using Milan.Simulation.Observers;

namespace Milan.Simulation.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ObserverIsNotReferencedInAnyModel : IDeleteRule
  {
    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as ISimulationObserver;
      if (target == null)
      {
        return;
      }
      var model = target.Model;
      model.Remove(target);
    }
  }
}