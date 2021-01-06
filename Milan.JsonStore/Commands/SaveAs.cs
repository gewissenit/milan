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
using Ork.Framework;

namespace Milan.JsonStore.Commands
{
  [Export(typeof (IShellCommand))]
  internal class SaveAs : IShellCommand
  {
    [ImportingConstructor]
    public SaveAs([Import] IJsonStore store)
    {
      Store = store;
      Name = "Save as";
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

      var saveFileDialog = new SaveFileDialog
                           {
                             Filter = "Projects (*.buis)|*.buis",
                             Title = "Save project",
                             InitialDirectory = Directory.Exists(settings.LastUsedPath)
                                                  ? settings.LastUsedPath
                                                  : settings.DefaultPath
                           };
      var fileChosen = saveFileDialog.ShowDialog();
      if (fileChosen != true)
      {
        return;
      }

      var fileName = saveFileDialog.FileName;

      Store.Save(fileName);
    }

    public string Image
    {
      get { return "appbar_disk_download"; }
    }

    public IDialogManager DialogManager { private get; set; }
  }
}