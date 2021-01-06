#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class AlignRight : MoveSelectedVisualsCommand
  {
    public AlignRight(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }


    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      var localCopy = selectedVisuals.ToArray();

      var right = localCopy.Max(x => x.Bounds.Right);

      foreach (var selectedVisual in localCopy)
      {
        selectedVisual.MoveHorizontallyTo(right - selectedVisual.Width);
      }
    }
  }
}