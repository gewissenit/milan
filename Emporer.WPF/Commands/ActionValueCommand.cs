#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows.Input;

namespace Emporer.WPF.Commands
{
  /// <summary>
  ///   ICommand-Implementierung mit zusätzlicher Value-Eigenschaft
  /// </summary>
  /// <typeparam name="TValue">Typ der Value-Eigenschaft</typeparam>
  public class ActionValueCommand<TValue> : ICommand
  {
    private readonly Action<TValue> _action;
    private bool _isEnabled = true;
    private TValue _value;

    public ActionValueCommand(Action<TValue> action, TValue value)
    {
      this._action = action;
      this._value = value;
    }

    public TValue Value
    {
      get { return _value; }
      set { this._value = value; }
    }

    public bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        _isEnabled = value;
        if (CanExecuteChanged != null)
        {
          CanExecuteChanged(this, EventArgs.Empty);
        }
      }
    }

    public bool CanExecute(object parameter)
    {
      return _isEnabled;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
      // parameter wird ignoriert und stattdessen der Wert von Value zurückgegeben
      _action(_value);
    }
  }
}