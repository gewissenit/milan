#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;

namespace Emporer.Material
{
  public interface ICategory : IDomainEntity
  {
    string Name { get; set; }
    string Description { get; set; }
    ICategory ParentCategory { get; set; }
  }
}