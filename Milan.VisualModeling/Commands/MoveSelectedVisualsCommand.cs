#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public abstract class MoveSelectedVisualsCommand : SelectionDependentCommand
  {
    protected MoveSelectedVisualsCommand(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }

    public override void Execute(object parameter)
    {
      MoveSelectedVisuals(VisualEditor.SelectedVisuals.OfType<IMovable>());
    }

    protected override bool IsExecutableFor(ISelectable selectedVisual)
    {
      return selectedVisual is IMovable;
    }

    protected override bool IsExecutableFor(IEnumerable<ISelectable> selectedVisuals)
    {
      return selectedVisuals.Count() > 1; // makes only sense for more than one item
    }

    protected abstract void MoveSelectedVisuals(IEnumerable<IMovable> selectedVisuals);
  }
}