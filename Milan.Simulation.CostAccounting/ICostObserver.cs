#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.Unit;
using Milan.Simulation.Observers;

namespace Milan.Simulation.CostAccounting
{
  public interface ICostObserver<TEntity> : ICostObserver, IEntityObserver<TEntity>
    where TEntity : IEntity
  {
  }

  public interface ICostObserver : IEntityObserver
  {
    double Amount { get; set; }
    double LossRatio { get; set; }
    string Category { get; set; }
    IUnit Currency { get; set; }
    IEnumerable<Position> BalancePositions { get; }
  }
}