#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Emporer.WPF.ViewModels;
using Milan.VisualModeling.Persistence;
using Milan.VisualModeling.Utilities;
using ReactiveUI;

namespace Milan.VisualModeling.ViewModels
{
  public class Node : Visual, INode
  {
    private const int AnchorPointMargin = 0;
    private readonly RelativeCoordinate _bottomConnectionAnchorPoint;
    private readonly VisualConfiguration _configuration;
    private readonly ObservableCollection<RelativeCoordinate> _connectionPorts = new ObservableCollection<RelativeCoordinate>();
    private readonly IViewModel _content;
    private readonly RelativeCoordinate _leftConnectionAnchorPoint;
    private readonly RelativeCoordinate _rightConnectionAnchorPoint;
    private readonly RelativeCoordinate _topConnectionAnchorPoint;

    public Node(IViewModel content, VisualConfiguration configuration)
    {
      _content = content;
      Model = content.Model;
      _configuration = configuration;

      _leftConnectionAnchorPoint = new RelativeCoordinate(Location);
      _topConnectionAnchorPoint = new RelativeCoordinate(Location);
      _rightConnectionAnchorPoint = new RelativeCoordinate(Location);
      _bottomConnectionAnchorPoint = new RelativeCoordinate(Location);

      _connectionPorts.Add(_leftConnectionAnchorPoint);
      _connectionPorts.Add(_topConnectionAnchorPoint);
      _connectionPorts.Add(_rightConnectionAnchorPoint);
      _connectionPorts.Add(_bottomConnectionAnchorPoint);

      Location.X = _configuration.X;
      Location.Y = _configuration.Y;
      Width = _configuration.Width;
      Height = _configuration.Height;
    }

    public IEnumerable<RelativeCoordinate> ConnectionAnchorPoints
    {
      get { return _connectionPorts; }
    }

    public IViewModel Content
    {
      get { return _content; }
    }

    public void MoveBy(Vector delta)
    {
      MoveBy(delta.X, delta.Y);
    }

    public void MoveBy(double deltaX, double deltaY)
    {
      MoveTo(Location.X + deltaX, Location.Y + deltaY);
    }

    public void MoveHorizontallyBy(double deltaX)
    {
      MoveHorizontallyTo(Location.X + deltaX);
    }

    public void MoveHorizontallyTo(double x)
    {
      ValueExtensions.ChangeButKeepAtLeastZero(() => Location.X, v => Location.X = v, x);
      _configuration.X = Location.X;
      this.RaisePropertyChanged("Location");
      UpdateAnchors();
    }

    public void MoveTo(Point location)
    {
      MoveTo(location.X, location.Y);
    }

    public void MoveTo(double x, double y)
    {
      ValueExtensions.ChangeButKeepAtLeastZero(() => Location.X, v => Location.X = v, x);
      ValueExtensions.ChangeButKeepAtLeastZero(() => Location.Y, v => Location.Y = v, y);
      _configuration.X = Location.X;
      _configuration.Y = Location.Y;
      this.RaisePropertyChanged("Location");
      UpdateAnchors();
    }

    public void MoveVerticallyBy(double deltaY)
    {
      MoveVerticallyTo(Location.Y + deltaY);
    }

    public void MoveVerticallyTo(double y)
    {
      ValueExtensions.ChangeButKeepAtLeastZero(() => Location.Y, v => Location.Y = v, y);
      _configuration.Y = Location.Y;
      this.RaisePropertyChanged("Location");
      UpdateAnchors();
    }

    public bool CanMoveTo(Point location)
    {
      return location.X > 0 && location.Y > 0;
    }

    public bool CanMoveBy(Vector delta)
    {
      return (Location.X + delta.X) > 0 && (Location.Y + delta.Y) > 0;
    }

    protected override void OnBoundsChanged()
    {
      base.OnBoundsChanged();
      UpdateAnchors();
    }

    public void ReactToKeyInput(EventArgs e)
    {
    }

    private void UpdateAnchors()
    {
      _leftConnectionAnchorPoint.OffsetY = Height / 2;
      _leftConnectionAnchorPoint.OffsetX = -AnchorPointMargin;

      _topConnectionAnchorPoint.OffsetX = Width / 2;
      _topConnectionAnchorPoint.OffsetY = -AnchorPointMargin;

      _rightConnectionAnchorPoint.OffsetX = Width + AnchorPointMargin;
      _rightConnectionAnchorPoint.OffsetY = Height / 2;

      _bottomConnectionAnchorPoint.OffsetX = Width / 2;
      _bottomConnectionAnchorPoint.OffsetY = Height + AnchorPointMargin;
    }
  }
}