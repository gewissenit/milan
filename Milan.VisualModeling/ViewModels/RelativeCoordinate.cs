#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using ReactiveUI;

namespace Milan.VisualModeling.ViewModels
{
  public class RelativeCoordinate : Coordinate
  {
    private readonly ICoordinate _reference;

    private double _offsetX;
    private double _offsetY;

    public RelativeCoordinate(ICoordinate reference, double offsetX = 0, double offsetY = 0)
    {
      _reference = reference;
      _offsetX = offsetX;
      _offsetY = offsetY;
      _reference.PropertyChanged += Update;
    }

    public double OffsetX
    {
      get { return _offsetX; }
      set
      {
        this.RaiseAndSetIfChanged(ref _offsetX, value);
        this.RaisePropertyChanged("X");
      }
    }

    public double OffsetY
    {
      get { return _offsetY; }
      set
      {
        this.RaiseAndSetIfChanged(ref _offsetY, value);
        this.RaisePropertyChanged("Y");
      }
    }

    public override double X
    {
      get { return _reference.X + _offsetX; }
    }

    public override double Y
    {
      get { return _reference.Y + _offsetY; }
    }

    public override string ToString()
    {
      return string.Format("[ {0:0.00} | {1:0.00} ] ( {2:0.00} + {3:0.00} | {4:0.00} + {5:0.00} )",
                           X,
                           Y,
                           _reference.X,
                           OffsetX,
                           _reference.Y,
                           OffsetY);
    }

    private void Update(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "X")
      {
        this.RaisePropertyChanged("X");
      }

      if (e.PropertyName == "Y")
      {
        this.RaisePropertyChanged("Y");
      }
    }
  }
}