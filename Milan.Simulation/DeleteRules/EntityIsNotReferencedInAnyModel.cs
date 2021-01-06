#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Milan.JsonStore;

namespace Milan.Simulation.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class EntityIsNotReferencedInAnyModel : IDeleteRule
  {
    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IEntity;

      if (target == null)
      {
        return;
      }

      var model = target.Model;
      model.Remove(target);
    }
  }
}