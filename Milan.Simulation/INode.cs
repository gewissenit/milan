#region License

// Copyright (c) 2013 HTW Berlin All rights reserved.

#endregion License

using System;
using System.Collections.Generic;

namespace Milan.Simulation
{
  public interface INode<T>
  {
    IEnumerable<T> Connections { get; }

    void Add(T connection);

    void Remove(T connection);

    event Action<INode<T>, T> Added;

    event Action<INode<T>, T> Removed;

    bool CanConnectToSource(INode<T> node);

    bool CanConnectToDestination(INode<T> node);
  }
}