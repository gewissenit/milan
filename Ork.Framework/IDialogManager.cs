﻿#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Caliburn.Micro;

namespace Ork.Framework
{
  public interface IDialogManager
  {
    void ShowDialog(IScreen dialogModel);
    void ShowMessageBox(string message, string title = null, MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null, MessageBoxOptions defaultOption=MessageBoxOptions.No|MessageBoxOptions.Cancel);
  }
}