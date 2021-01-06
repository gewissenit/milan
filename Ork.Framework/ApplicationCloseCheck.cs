#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Ork.Framework
{
  public class ApplicationCloseCheck : IResult
  {
    private readonly Action<IDialogManager, Action<bool>> _closeCheck;
    private readonly IChild _screen;

    public ApplicationCloseCheck(IChild screen, Action<IDialogManager, Action<bool>> closeCheck)
    {
      _screen = screen;
      _closeCheck = closeCheck;
    }

    [Import]
    public IShell Shell
    {
      get;
      set;
    }

    public event EventHandler<ResultCompletionEventArgs> Completed = delegate
                                                                     {
                                                                     };

    public void Execute(CoroutineExecutionContext context)
    {
      var documentWorkspace = _screen.Parent as IDocumentWorkspace;
      if (documentWorkspace != null)
      {
        documentWorkspace.Edit(_screen);
      }
      else
      {
        Shell.ActivateItem(_screen);
      }

      _closeCheck(Shell.Dialogs,
                   result => Completed(this,
                                       new ResultCompletionEventArgs
                                       {
                                         WasCancelled = !result
                                       }));
    }
  }
}