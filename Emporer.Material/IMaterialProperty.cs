#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Material
{
  public interface IMaterialProperty
  {
    IPropertyType PropertyType { get; set; }
    double Mean { get; set; }
  }
}