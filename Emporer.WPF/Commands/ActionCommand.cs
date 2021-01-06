#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows.Input;

namespace Emporer.WPF.Commands
{
  public class ActionCommand : ICommand
  {
    private readonly Action<object> _action;

    public ActionCommand(Action<object> action)
    {
      _action = action;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
      _action(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
      if (CanExecuteChanged != null)
      {
        CanExecuteChanged(this, new EventArgs());
      }
    }
  }
}