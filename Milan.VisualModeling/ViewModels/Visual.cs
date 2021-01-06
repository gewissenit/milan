#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Milan.VisualModeling.Extensions;
using ReactiveUI;

namespace Milan.VisualModeling.ViewModels
{
  public abstract class Visual : ReactiveObject, ISelectable, IVisual
  {
    private Rect _bounds;
    private Color _color;
    private double _height;
    private bool _isSelected;
    private string _name;
    private double _width;

    public Visual()
    {
      Location = new Coordinate();
      Location.PropertyChanged += UpdateBounds;
    }

    public Color Color
    {
      get { return _color; }
      set { this.RaiseAndSetIfChanged(ref _color, value); }
    }

    public string Name
    {
      get { return _name; }
      set { this.RaiseAndSetIfChanged(ref _name, value); }
    }

    public bool IsSelected
    {
      get { return _isSelected; }
      set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
    }

    public virtual Rect Bounds
    {
      get { return _bounds; }
      private set
      {
        this.RaiseAndSetIfChanged(ref _bounds, value);
      }
    }

    public object Model { get; protected set; }

    public double Height
    {
      get { return _height; }
      set
      {
        this.RaiseAndSetIfChanged(ref _height, value);
        UpdateBounds();
      }
    }

    public ICoordinate Location { get; protected set; }

    public double Width
    {
      get { return _width; }
      set
      {
        UpdateBounds();
        this.RaiseAndSetIfChanged(ref _width, value);
      }
    }

    private void UpdateBounds(object sender, PropertyChangedEventArgs e)
    {
      UpdateBounds();
    }

    private void UpdateBounds()
    {
      Bounds = new Rect(Location.ToPoint(), new Size(Width, Height));
      OnBoundsChanged();
    }

    protected virtual void OnBoundsChanged()
    {
    }
  }
}