#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Ork.Framework
{
  public enum MessageBoxOptions
  {
    Ok = 2,
    Cancel = 4,
    Yes = 8,
    No = 16,

    OkCancel = Ok | Cancel,
    YesNo = Yes | No,
    YesNoCancel = Yes | No | Cancel
  }
}