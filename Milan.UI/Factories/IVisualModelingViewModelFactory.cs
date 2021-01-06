#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation;
using Milan.VisualModeling.ViewModels;

namespace Milan.UI.Factories
{
  public interface IVisualModelingViewModelFactory
  {
    IEdge CreateEdge(INode source, INode destination);
    INode CreateNode(IEntity entity);
  }
}