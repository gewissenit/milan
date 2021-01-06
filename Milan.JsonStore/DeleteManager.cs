#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Milan.JsonStore
{
  [Export(typeof (IDeleteManager))]
  internal class DeleteManager : IDeleteManager
  {
    [ImportMany]
    private IEnumerable<IDeleteRule> Rules { get; set; }

    [Import]
    private IJsonStore Store { get; set; }

    public void Delete(object domainEntity)
    {
      Rules.ForEach(r => r.CleanReferences(domainEntity));
      Store.Remove(domainEntity);
    }
  }
}