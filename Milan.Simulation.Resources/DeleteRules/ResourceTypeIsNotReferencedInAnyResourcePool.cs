#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;

namespace Milan.Simulation.Resources.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ResourceTypeIsNotReferencedInAnyResourcePool : IDeleteRule
  {
    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IResourceType;
      if (target == null)
      {
        return;
      }
      var model = target.Model;
      foreach (var rp in model.Entities.OfType<IResourcePool>()
                              .Where(ws => ws.Resources.SingleOrDefault(rta => rta.ResourceType == target) != null))
      {
        rp.Remove(rp.Resources.SingleOrDefault(rta => rta.ResourceType == target));
      }
    }
  }
}