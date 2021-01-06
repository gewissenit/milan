#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Ork.Framework.Properties;
using System.Windows.Input;

namespace Ork.Framework.ViewModels
{
  [Export(typeof (IShell))]
  public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive, IShell
  {
    [ImportingConstructor]
    public ShellViewModel([ImportMany] IEnumerable<IWorkspace> workspaces,
                          [Import] IDialogManager dialogManager,
                          [Import("ApplicationName")] string appName)

    {
      DisplayName = appName;
      Dialogs = dialogManager;
      Items.AddRange(workspaces.OrderBy(index => index.Index));

      foreach (var item in Items)
      {
        item.Parent = this;
      }

      ActivateItem(Items[1]);
      CloseStrategy = new ApplicationCloseStrategy();
#if DEMO
      CheckLicense();
#endif
    }

    public IDialogManager Dialogs { get; private set; }

    private void CheckLicense()
    {
      if (Settings.Default.AppRunTime > TimeSpan.FromDays(0.5))
      {
        CloseStrategy = new ImmediatelyCloseStrategy();
        var text = "###Thank you for using EcoFactory. " + Environment.NewLine;
        text +=
          "The trial perriod of 10 days is exceeded. If you further want to use EcoFactory please buy it by contacting us via the geWISSEN website (http://www.gewissen-it.de/contact) or via info@gewissen-it.de.";
        Dialogs.ShowMessageBox(text, "TRIAL PERIOD EXCEEDED", MessageBoxOptions.Ok, CloseApplication);
      }
    }

    private void CloseApplication(IMessageBox messageBox)
    {
      TryClose();
    }

    public void HandleKeyInput(Key key)
    {
      ActiveItem.HandleKeyInput(key);
    }
  }
}