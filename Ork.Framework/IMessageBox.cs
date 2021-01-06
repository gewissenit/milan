#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Ork.Framework
{
  public interface IMessageBox : IScreen
  {
    string Message { get; set; }

    MessageBoxOptions Options { get; set; }
    MessageBoxOptions DefaultOption { get; set; }

    void Cancel();

    void No();
    void Ok();

    bool WasSelected(MessageBoxOptions option);
    void Yes();
  }
}