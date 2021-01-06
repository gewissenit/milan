#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Ork.Framework;

namespace Milan.JsonStore.Commands
{
  [Export(typeof (IShellCommand))]
  internal class New : IShellCommand
  {
    [ImportingConstructor]
    public New([Import] IJsonStore store)
    {
      Store = store;
      Name = "New";
    }

    public IJsonStore Store { get; set; }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      if (Store.UnsavedChanges)
      {
        DialogManager.ShowMessageBox("There are unsaved changes. Creating a new file will loose that changes. Do you really want to do this?",
                                     "LOOSE UNSAVED CHANGES",
                                     MessageBoxOptions.YesNo,
                                     EvaluateLooseChangesDecision);
      }
      else
      {
        NewFile();
      }
    }

    public string Name { get; private set; }

    public string Image
    {
      get { return "appbar_page_new"; }
    }

    public IDialogManager DialogManager { private get; set; }

    private void NewFile()
    {
      try
      {
        Store.New();
      }
      catch (Exception)
      {
        DialogManager.ShowMessageBox("Unknown exception occured while creating new project.", "ERROR");
      }
    }

    private void EvaluateLooseChangesDecision(IMessageBox obj)
    {
      if (obj.WasSelected(MessageBoxOptions.Yes))
      {
        NewFile();
      }
    }
  }
}