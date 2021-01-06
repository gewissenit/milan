#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Milan.JsonStore;

namespace Emporer.Material
{
  public interface ICategorizable : IDomainEntity
  {
    IEnumerable<ICategory> Categories { get; }
    void Add(ICategory category);
    void Remove(ICategory category);
    event Action<ICategorizable, ICategory> Added;
    event Action<ICategorizable, ICategory> Removed;
  }
}