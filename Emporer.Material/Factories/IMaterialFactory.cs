#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Material.Factories
{
  public interface IMaterialFactory
  {
    IMaterial Create();
    IMaterial Duplicate(IMaterial material);
  }
}