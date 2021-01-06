#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public class DistributeHorizontally : MoveSelectedVisualsCommand
  {
    public DistributeHorizontally(VisualEditor visualEditor)
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
      var totalLeft = visuals.Min(x => x.Bounds.Left);
      var totalRight = visuals.Max(x => x.Bounds.Right);
      var totalWidth = totalRight - totalLeft;
      var totalOccupiedWidth = visuals.Sum(x => x.Width);
      var totalAvailableSpace = totalWidth - totalOccupiedWidth;
      var space = totalAvailableSpace / (visuals.Length - 1);

      var left = totalLeft;
      foreach (var movable in visuals.OrderBy(x => x.Bounds.Left))
      {
        movable.MoveHorizontallyTo(left);
        left += movable.Width;
        left += space;
      }
    }
  }
}