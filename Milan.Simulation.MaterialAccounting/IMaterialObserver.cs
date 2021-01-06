#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.Material;
using Emporer.Unit;
using Milan.Simulation.MaterialAccounting.ReportDataProviders;
using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting
{
  public interface IMaterialObserver : IEntityObserver
  {
    double Amount { get; set; }
    double LossRatio { get; set; }
    string Category { get; set; }
    IMaterial Material { get; set; }
    IUnit Unit { get; set; }
    IEnumerable<MaterialPosition> BalancePositions { get; }
  }

  public interface IMaterialObserver<TEntity> : IMaterialObserver, IEntityObserver<TEntity>
    where TEntity : IEntity
  {
  }
}