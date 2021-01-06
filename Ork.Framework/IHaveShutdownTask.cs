#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Ork.Framework
{
  public interface IHaveShutdownTask
  {
    IResult GetShutdownTask();
  }
}