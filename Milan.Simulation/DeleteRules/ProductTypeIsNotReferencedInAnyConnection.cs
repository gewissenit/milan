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
  internal class ProductTypeIsNotReferencedInAnyConnection : IDeleteRule
  {
    public void CleanReferences(object domainEntity)
    {
      var target = domainEntity as IProductType;
      if (target == null)
      {
        return;
      }
      foreach (var connection in target.Model.Entities.OfType<IStationaryElement>()
                                       .Where(se => se.Connections.Any(con => con.ProductTypes.Contains(target)))
                                       .SelectMany(entity => entity.Connections.Where(con => con.ProductTypes.Contains(target))))
      {
        connection.Remove(target);
        if (!connection.ProductTypes.Any())
        {
          connection.IsRoutingPerProductType = false;
        }
      }
    }
  }
}