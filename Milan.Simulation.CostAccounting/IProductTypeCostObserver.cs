#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Observers;

namespace Milan.Simulation.CostAccounting
{
  public interface IProductTypeCostObserver<TEntity> : ICostObserver<TEntity>, IProductRelated
    where TEntity : IEntity
  {
  }
}