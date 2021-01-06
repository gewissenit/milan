#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows.Input;

namespace Ork.Framework
{
  public interface IShellCommand : ICommand
  {
    string Name { get; }
    string Image { get; }
    IDialogManager DialogManager { set; }
  }
}