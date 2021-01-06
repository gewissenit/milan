#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows.Input;
using GeWISSEN.Utils;

namespace Emporer.WPF.Commands
{
  public class RelayCommand : ICommand
  {
    private readonly Action<object> _execute;
    private readonly Predicate<object> _isExecutionPossible;
    private bool _canExecute;

    public RelayCommand(Action<object> execute)
      : this(execute, o => true)
    {
    }

    public RelayCommand(Action<object> execute, Predicate<object> isExecutionPossible)
    {
      Throw.IfNull(isExecutionPossible, nameof(isExecutionPossible));

      _canExecute = false;
      _execute = execute;
      _isExecutionPossible = isExecutionPossible;
    }

    public event EventHandler CanExecuteChanged;


    public bool CanExecute(object parameter)
    {
      return _isExecutionPossible(parameter);
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    public void EvaluateCanExecute(object value)
    {
      var canExecute = _isExecutionPossible(value);

      if (_canExecute == canExecute)
      {
        return;
      }
      _canExecute = canExecute;
      ChangeCanExecute();
    }

    public void UpdateCanExecute()
    {
      ChangeCanExecute();
    }

    private void ChangeCanExecute()
    {
      var canExecuteChanged = CanExecuteChanged;
      canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}