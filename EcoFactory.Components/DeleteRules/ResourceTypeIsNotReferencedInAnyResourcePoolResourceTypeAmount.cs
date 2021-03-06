﻿#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation.Resources;

namespace EcoFactory.Components.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class ResourceTypeIsNotReferencedInAnyResourcePoolResourceTypeAmount : IDeleteRule
  {
    [Import]
    private IDeleteManager DeleteManager { get; set; }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IResourceType;
      if (target == null)
      {
        return;
      }
      var model = target.Model;
      foreach (var rta in model.Entities.OfType<IWorkstationBase>()
                                               .SelectMany(ws => ws.ProcessingResources
                                                                   .Concat(ws.SetupResources)
                                                                   .Where(rta => rta.ResourceType == target)
                                                                   .ToArray()))

      {
        DeleteManager.Delete(rta);
      }
    }
  }
}