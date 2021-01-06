#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Ork.Framework
{
  public class DocumentBase : Screen, IHaveShutdownTask
  {
    private bool _isDirty;

    [Import]
    public virtual IDialogManager Dialogs { get; set; }

    public virtual bool IsDirty
    {
      get { return _isDirty; }
      set
      {
        _isDirty = value;
        NotifyOfPropertyChange(() => IsDirty);
      }
    }

    public IResult GetShutdownTask()
    {
      return IsDirty
               ? new ApplicationCloseCheck(this, DoCloseCheck)
               : null;
    }

    public override void CanClose(Action<bool> callback)
    {
      if (IsDirty)
      {
        DoCloseCheck(Dialogs, callback);
      }
      else
      {
        callback(true);
      }
    }

    protected virtual void DoCloseCheck(IDialogManager dialogs, Action<bool> callback)
    {
      dialogs.ShowMessageBox("There are unsaved changes. Closing EcoFactory will loose that changes. Do you really want to do this?",
                             "LOOSE UNSAVED CHANGES",
                             MessageBoxOptions.YesNo,
                             box => callback(box.WasSelected(MessageBoxOptions.Yes)),
                             MessageBoxOptions.No);
    }
  }
}