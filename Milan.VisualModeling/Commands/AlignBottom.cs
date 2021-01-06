#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class AlignBottom : MoveSelectedVisualsCommand
  {
    public AlignBottom(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }


    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      var localCopy = selectedVisuals.ToArray();

      var bottom = localCopy.Max(x => x.Bounds.Bottom);

      foreach (var selectedVisual in localCopy)
      {
        selectedVisual.MoveVerticallyTo(bottom - selectedVisual.Height);
      }
    }
  }
}