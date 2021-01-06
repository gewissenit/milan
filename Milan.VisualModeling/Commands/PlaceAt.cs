#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.VisualModeling.ViewModels;
using ReactiveUI;

namespace Milan.VisualModeling.Commands
{
  public class PlaceAt : MoveSelectedVisualsCommand
  {
    private int _X;
    private int _Y;

    public PlaceAt(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }

    public int X
    {
      get { return _X; }
      private set { this.RaiseAndSetIfChanged(ref _X, value); }
    }

    public int Y
    {
      get { return _Y; }
      private set { this.RaiseAndSetIfChanged(ref _Y, value); }
    }

    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      foreach (var selectedVisual in selectedVisuals)
      {
        selectedVisual.MoveTo(X, Y);
      }
    }
  }
}