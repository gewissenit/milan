#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;

namespace Milan.Simulation.DeleteRules
{
  [Export(typeof (IDeleteRule))]
  internal class StationaryElementIsNotReferencedInAnyConnection : IDeleteRule
  {
    [Import]
    private IDeleteManager DeleteManager { get; set; }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IStationaryElement;
      if (target == null)
      {
        return;
      }
      foreach (var connection in target.Model.Entities.OfType<IStationaryElement>()
                                       .Where(se => se.Connections.Any(w => w.Destination == target))
                                       .Select(predeccessor => predeccessor.Connections.Single(con => con.Destination == target)).ToArray())
      {
        DeleteManager.Delete(connection);
      }

      foreach (var connection in target.Connections.ToArray())
      {
        DeleteManager.Delete(connection);
      }
    }
  }
}