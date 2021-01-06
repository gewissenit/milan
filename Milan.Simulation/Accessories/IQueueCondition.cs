#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Accessories
{
  public interface IQueueCondition<T>
    where T : class, IEntity
  {
    bool IsMetBy(T entity);
  }
}