#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class AlignVerticallyCentered : MoveSelectedVisualsCommand
  {
    public AlignVerticallyCentered(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }


    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      var localCopy = selectedVisuals.ToArray();

      var top = localCopy.Min(x => x.Bounds.Top);
      var bottom = localCopy.Max(x => x.Bounds.Bottom);
      var center = bottom - ((bottom - top) / 2);

      foreach (var selectedVisual in localCopy)
      {
        selectedVisual.MoveVerticallyTo(center - selectedVisual.Height / 2);
      }
    }
  }
}