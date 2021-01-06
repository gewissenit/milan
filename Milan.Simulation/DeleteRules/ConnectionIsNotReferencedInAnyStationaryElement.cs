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
  internal class ConnectionIsNotReferencedInAnyStationaryElement : IDeleteRule
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public ConnectionIsNotReferencedInAnyStationaryElement([Import] IJsonStore store)
    {
      _store = store;
    }

    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IConnection;
      if (target == null)
      {
        return;
      }

      foreach (var stationaryElement in _store.Content.OfType<IModel>()
                                               .SelectMany(m => m.Entities)
                                               .OfType<IStationaryElement>()
                                               .Where(stationaryElement => stationaryElement.Connections.Contains(target)))
      {
        stationaryElement.Remove(target);
        break;
      }
    }
  }
}