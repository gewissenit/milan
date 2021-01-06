#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Ork.Framework
{
  public interface IDocumentWorkspace : IWorkspace
  {
    void Edit(object document);
  }
}