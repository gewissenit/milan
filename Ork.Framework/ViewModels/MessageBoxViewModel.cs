#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using GeWISSEN.Utils;
using Action = System.Action;

namespace Ork.Framework.ViewModels
{
  [Export(typeof (IMessageBox))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class MessageBoxViewModel : Screen, IMessageBox
  {
    private static readonly Map<MessageBoxOptions, MessageBoxOptions> _positiveOptions;
    private static readonly Map<MessageBoxOptions, MessageBoxOptions> _negativeOptions;

    private readonly Map<Key, Action> _keyBindings;

    private MessageBoxOptions _selection;

    static MessageBoxViewModel()
    {
      _positiveOptions = new Map<MessageBoxOptions, MessageBoxOptions>()
                         {
                           {
                             MessageBoxOptions.OkCancel, MessageBoxOptions.Ok
                           },
                           {
                             MessageBoxOptions.YesNo, MessageBoxOptions.Yes
                           },
                           {
                             MessageBoxOptions.YesNoCancel, MessageBoxOptions.Yes
                           }
                         };
      _negativeOptions = new Map<MessageBoxOptions, MessageBoxOptions>()
                         {
                           {
                             MessageBoxOptions.OkCancel, MessageBoxOptions.Cancel
                           },
                           {
                             MessageBoxOptions.YesNo, MessageBoxOptions.No
                           },
                           {
                             MessageBoxOptions.YesNoCancel, MessageBoxOptions.Cancel
                           }
                         };
    }

    public MessageBoxViewModel()
    {
      _keyBindings = new Map<Key, Action>(Do.Nothing)
                     {
                       {
                         Key.Enter, SelectPositiveOption
                       },
                       {
                         Key.Escape, SelectNegativeOption
                       },
                       {
                         Key.Left, SelectPositiveOption
                       },
                       {
                         Key.Right, SelectNegativeOption
                       }
                     };
    }

    public bool CancelVisible
    {
      get { return IsVisible(MessageBoxOptions.Cancel); }
    }

    public bool NoVisible
    {
      get { return IsVisible(MessageBoxOptions.No); }
    }

    public bool OkVisible
    {
      get { return IsVisible(MessageBoxOptions.Ok); }
    }

    public bool YesVisible
    {
      get { return IsVisible(MessageBoxOptions.Yes); }
    }

    public MessageBoxOptions SelectedOption
    {
      get { return _selection; }

      set
      {
        if (_selection == value)
        {
          return;
        }
        _selection = value;
        NotifyOfPropertyChange(() => SelectedOption);
        Console.WriteLine("MB selected option {0}", _selection);
      }
    }

    public string Message { get; set; }
    public MessageBoxOptions Options { get; set; }
    public MessageBoxOptions DefaultOption { get; set; }

    public void Cancel()
    {
      Select(MessageBoxOptions.Cancel);
    }

    public void No()
    {
      Select(MessageBoxOptions.No);
    }

    public void Ok()
    {
      Select(MessageBoxOptions.Ok);
    }

    public bool WasSelected(MessageBoxOptions option)
    {
      return (_selection & option) == option;
    }

    public void Yes()
    {
      Select(MessageBoxOptions.Yes);
    }

    public void HandleKeyInput(Key key)
    {
      Console.WriteLine("MB received PKD ({0})", key);
      _keyBindings[key]();
    }

    public void SetFocusToPreferredOption()
    {
      Console.WriteLine("MB VM: Selecting default option ({0})...", DefaultOption);
      SelectedOption = DefaultOption;
    }

    private void SelectNegativeOption()
    {
      SelectedOption = _negativeOptions[Options];
    }

    private void SelectPositiveOption()
    {
      SelectedOption = _positiveOptions[Options];
    }

    private bool IsVisible(MessageBoxOptions option)
    {
      return (Options & option) == option;
    }

    private void Select(MessageBoxOptions option)
    {
      _selection = option;
      TryClose();
    }
  }
}