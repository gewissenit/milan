#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Accessories
{
  internal class CategorizedEntity<TEntity, TCategory>
    where TEntity : class
  {
    private readonly TCategory _category;
    private readonly TEntity _entity;

    internal CategorizedEntity(TEntity entity, TCategory category)
    {
      _entity = entity;
      _category = category;
    }

    internal TEntity Entity
    {
      get { return _entity; }
    }

    internal TCategory Category
    {
      get { return _category; }
    }
  }
}