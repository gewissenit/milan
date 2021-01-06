#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;

namespace Emporer.Unit
{
  public interface IUnit : IDomainEntity
  {
    double Coefficient { get; set; }
    string Name { get; set; }
    string Dimension { get; set; }
    string Symbol { get; set; }
    IUnit ReferencedUnit { get; set; }
    bool IsBasicUnit { get; }
    IUnit BasicUnit { get; }
    double ToBaseUnit(double valueInThisUnit);
    double FromBaseUnit(double valueInBaseUnit);
    bool IsConvertibleTo(IUnit other);
  }
}