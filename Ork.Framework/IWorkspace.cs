#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using System.Windows.Input;

namespace Ork.Framework
{
  public interface IWorkspace: IScreen, IChild
  {
    int Index { get; }
    bool IsEnabled { get; }
    void HandleKeyInput(Key key);
  }
}