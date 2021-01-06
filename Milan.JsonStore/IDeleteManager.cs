#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.JsonStore
{
  public interface IDeleteManager
  {
    void Delete(object domainEntity);
  }
}