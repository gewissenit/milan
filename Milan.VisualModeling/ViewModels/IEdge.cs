#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.VisualModeling.ViewModels
{
  public interface IEdge : IVisual
  {
    INode Destination { get; set; }
    ICoordinate DestinationAnchor { get; set; }
    INode Source { get; set; }
    ICoordinate SourceAnchor { get; set; }
  }
}