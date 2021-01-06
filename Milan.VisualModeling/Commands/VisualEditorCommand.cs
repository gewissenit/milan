#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows.Input;
using ReactiveUI;

namespace Milan.VisualModeling.Commands
{
  public abstract class VisualEditorCommand : ReactiveObject, ICommand
  {
    protected readonly VisualEditor VisualEditor;
    private bool _IsExecutable;

    public VisualEditorCommand(VisualEditor visualEditor)
    {
      VisualEditor = visualEditor;
    }

    protected bool IsExecutable
    {
      get { return _IsExecutable; }
      set
      {
        if (_IsExecutable == value)
        {
          return;
        }
        _IsExecutable = value;
        RaiseCanExecuteChanged();
      }
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return IsExecutable;
    }

    public abstract void Execute(object parameter);

    private void RaiseCanExecuteChanged()
    {
      if (CanExecuteChanged == null)
      {
        return;
      }
      CanExecuteChanged(this, new EventArgs());
    }
  }
}