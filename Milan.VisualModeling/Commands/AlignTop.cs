#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class AlignTop : MoveSelectedVisualsCommand
  {
    public AlignTop(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }


    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      var localCopy = selectedVisuals.ToArray();

      var top = localCopy.Min(x => x.Bounds.Top);

      foreach (var selectedVisual in localCopy)
      {
        selectedVisual.MoveVerticallyTo(top);
      }
    }
  }
}