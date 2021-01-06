#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Emporer.Unit.Factories
{
  public interface IStandardUnitFactory
  {
    IEnumerable<IUnit> StandardUnits { get; }
  }
}