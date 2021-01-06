#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using Milan.JsonStore.Properties;
using Ork.Framework;

namespace Milan.JsonStore.Commands
{
  [Export(typeof (IShellCommand))]
  internal class Save : IShellCommand
  {
    [ImportingConstructor]
    public Save([Import] IJsonStore store)
    {
      Store = store;
      Name = "Save";
    }

    public IJsonStore Store { get; set; }
    public string Name { get; private set; }


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
      var settings = Settings.Default;
      if (string.IsNullOrEmpty(settings.LastSaveFile) ||
          !File.Exists(settings.LastSaveFile))
      {
        var saveAs = new SaveAs(Store);
        saveAs.Execute(parameter);
      }
      else
      {
        try
        {
          Store.Save();
        }
        catch (Exception)
        {
          DialogManager.ShowMessageBox("Unknown exception occured while saving.", "ERROR");
        }
      }
    }

    public string Image
    {
      get
      {
        return "appbar_disk";
      }
    }

    public IDialogManager DialogManager
    {
      private get;
      set;
    }
  }
}