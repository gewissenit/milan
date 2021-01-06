#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Unit
{
  public interface IValue
  {
    double Amount { get; set; }
    IUnit Unit { get; }
  }
}