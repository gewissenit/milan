#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.WPF.ViewModels;

namespace Milan.VisualModeling.ViewModels
{
  public interface INode : ISelectable, IMovable
  {
    IViewModel Content { get; }
    IEnumerable<RelativeCoordinate> ConnectionAnchorPoints { get; }
  }
}