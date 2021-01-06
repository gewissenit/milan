#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.VisualModeling.ViewModels;

namespace Milan.VisualModeling.Commands
{
  public abstract class SelectionDependentCommand : VisualEditorCommand, IDisposable
  {
    private readonly IDisposable _selectionChangeSubscription;

    protected SelectionDependentCommand(VisualEditor visualEditor)
      : base(visualEditor)
    {
      if (visualEditor == null)
      {
        throw new ArgumentNullException();
      }

      IsExecutable = VisualEditor.SelectedVisuals.Any();
      _selectionChangeSubscription = VisualEditor.SelectionChanges.Subscribe(DetermineIfIsExecutable);
    }

    public void Dispose()
    {
      _selectionChangeSubscription.Dispose();
    }

    protected virtual bool IsExecutableFor(ISelectable selectedVisual)
    {
      return true;
    }

    protected virtual bool IsExecutableFor(IEnumerable<ISelectable> selectedVisuals)
    {
      return true;
    }

    private void DetermineIfIsExecutable(IEnumerable<ISelectable> selectedVisuals)
    {
      IsExecutable = IsExecutableFor(selectedVisuals) && selectedVisuals.Where(IsExecutableFor)
                                                                        .Any();
    }
  }
}