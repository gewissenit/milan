#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class DistributeVertically : MoveSelectedVisualsCommand
  {
    public DistributeVertically(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }

    protected override bool IsExecutableFor(IEnumerable<ISelectable> selectedVisuals)
    {
      return selectedVisuals.Count() > 2; // makes only sense for more than two items
    }

    protected override void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals)
    {
      var visuals = selectedVisuals.ToArray();
      var totalTop = visuals.Min(x => x.Bounds.Top);
      var totalBottom = visuals.Max(x => x.Bounds.Bottom);
      var totalHeight = totalBottom - totalTop;
      var totalOccupiedHeight = visuals.Sum(x => x.Height);
      var totalAvailableSpace = totalHeight - totalOccupiedHeight;
      var space = totalAvailableSpace / (visuals.Length - 1);

      var top = totalTop;
      foreach (var movable in visuals.OrderBy(x => x.Bounds.Top))
      {
        movable.MoveVerticallyTo(top);
        top += movable.Height;
        top += space;
      }
    }
  }
}