#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using Milan.JsonStore.Properties;
using Newtonsoft.Json;
using Ork.Framework;

namespace Milan.JsonStore.Commands
{
  [Export(typeof (IShellCommand))]
  internal class Open : IShellCommand
  {
    [ImportingConstructor]
    public Open([Import] IJsonStore store)
    {
      Store = store;
      Name = "Open";
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
      if (Store.UnsavedChanges)
      {
        DialogManager.ShowMessageBox("There are unsaved changes. Opening a file will loose that changes. Do you really want to do this?",
                                     "LOOSE UNSAVED CHANGES",
                                     MessageBoxOptions.YesNo,
                                     EvaluateLooseChangesDecision);
      }
      else
      {
        OpenFile();
      }
    }

    public string Image
    {
      get { return "appbar_folder_open"; }
    }

    public IDialogManager DialogManager { private get; set; }

    private void OpenFile()
    {
      var settings = Settings.Default;

      var openFileDialog = new OpenFileDialog()
                           {
                             Filter = "Projects (*.buis)|*.buis",
                             Title = "Open project",
                             InitialDirectory = Directory.Exists(settings.LastUsedPath)
                                                  ? settings.LastUsedPath
                                                  : settings.DefaultPath
                           };
      var fileChosen = openFileDialog.ShowDialog();
      if (fileChosen != true)
      {
        return;
      }

      var fileName = openFileDialog.FileName;
      try
      {
        Store.Load(fileName);
      }
      catch (JsonSerializationException e)
      {
        var message = "Cannot open file. It is not compatible with current version.";
        message += Environment.NewLine;
        message += string.Format("Details: {0}", e.Message);
        message += Environment.NewLine;
        message += "Open another file or create a new project.";
        DialogManager.ShowMessageBox(message, "ERROR");
      }
    }

    private void EvaluateLooseChangesDecision(IMessageBox obj)
    {
      if (obj.WasSelected(MessageBoxOptions.Yes))
      {
        OpenFile();
      }
    }
  }
}