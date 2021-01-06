#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using Milan.VisualModeling.Extensions;
using ReactiveUI;

namespace Milan.VisualModeling.ViewModels
{
  public class Edge : Visual, IEdge
  {
    private readonly ISnapBehaviour _snapping;
    private INode _destination;
    private ICoordinate _destinationAnchor;
    private INode _source;
    private ICoordinate _sourceAnchor;

    public Edge(INode source, INode destination, object model, ISnapBehaviour snapping)
    {
      if (source == null)
      {
        throw new ArgumentNullException();
      }

      _snapping = snapping;
      Model = model;

      Source = source;
      if (destination == null)
      {
        destination = source;
      }
      Destination = destination;

      FindAppropriateAnchors();
    }

    public INode Destination
    {
      get { return _destination; }
      set
      {
        this.RaiseAndSetIfChanged(ref _destination, value);
        if (Source != null && Destination != null)
        {
          DestinationAnchor = Source.Location.GetNearest(Destination.ConnectionAnchorPoints);
        }
      }
    }

    public ICoordinate DestinationAnchor
    {
      get { return _destinationAnchor; }
      set
      {
        if (_destinationAnchor != null)
        {
          _destinationAnchor.PropertyChanged -= UpdateBounds;
        }

        this.RaiseAndSetIfChanged(ref _destinationAnchor, value);
        _destinationAnchor.PropertyChanged += UpdateBounds;
      }
    }

    public INode Source
    {
      get { return _source; }
      set
      {
        this.RaiseAndSetIfChanged(ref _source, value);
        if (Source != null && Destination != null)
        {
          SourceAnchor = Destination.Location.GetNearest(Source.ConnectionAnchorPoints);
        }
      }
    }

    public ICoordinate SourceAnchor
    {
      get { return _sourceAnchor; }
      set
      {
        if (_sourceAnchor != null)
        {
          _sourceAnchor.PropertyChanged -= UpdateBounds;
        }

        this.RaiseAndSetIfChanged(ref _sourceAnchor, value);
        _sourceAnchor.PropertyChanged += UpdateBounds;
      }
    }

    private void UpdateBounds(object sender, PropertyChangedEventArgs e)
    {
      UpdateBounds();
    }

    private void FindAppropriateAnchors()
    {
      var centerOfDestination = Destination.Bounds.GetCenter();
      var centerOfSource = Source.Bounds.GetCenter();

      SourceAnchor = centerOfDestination.ToCoordinate()
                                        .GetNearest(Source.ConnectionAnchorPoints);

      DestinationAnchor = centerOfSource.ToCoordinate()
                                             .GetNearest(Destination.ConnectionAnchorPoints);
    }

    private void UpdateBounds()
    {
      FindAppropriateAnchors();

      // location will be upper left corner of the bounding rectangle
      Location.X = _sourceAnchor.X <= _destinationAnchor.X
                     ? _sourceAnchor.X
                     : _destinationAnchor.X;
      Location.Y = _sourceAnchor.Y <= _destinationAnchor.Y
                     ? _sourceAnchor.Y
                     : _destinationAnchor.Y;
      Width = Math.Abs(_destinationAnchor.X - _sourceAnchor.X);
      Height = Math.Abs(_destinationAnchor.Y - _sourceAnchor.Y);
    }
  }
}