#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Math.Distribution
{
  public interface IConfigurable<T>
  {
    void Configure(T distributionFactory);
  }
}