#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Ork.Framework
{
  public class ImmediatelyCloseStrategy: ICloseStrategy<IWorkspace>
  {
    public void Execute(IEnumerable<IWorkspace> toClose, Action<bool, IEnumerable<IWorkspace>> cb)
    {
      cb(true, new List<IWorkspace>());
    }
  }
}