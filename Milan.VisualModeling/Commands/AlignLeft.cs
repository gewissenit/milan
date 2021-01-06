#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class AlignLeft : MoveSelectedVisualsCommand
  {
    public AlignLeft(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }


    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      var localCopy = selectedVisuals.ToArray();

      var left = localCopy.Min(x => x.Bounds.Left);

      foreach (var selectedVisual in localCopy)
      {
        selectedVisual.MoveHorizontallyTo(left);
      }
    }
  }
}