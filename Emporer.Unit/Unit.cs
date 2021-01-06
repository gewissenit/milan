#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;
using Newtonsoft.Json;

namespace Emporer.Unit
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Unit : DomainEntity, IUnit
  {
    [JsonProperty]
    public double Coefficient
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Name
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Dimension
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Symbol
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IUnit ReferencedUnit
    {
      get { return Get<IUnit>(); }
      set { Set(value); }
    }

    /// <summary>
    ///   Converts a value into the base unit of this unit.
    /// </summary>
    /// <param name="valueInThisUnit">The value in this unit.</param>
    /// <returns>
    ///   A value in the base unit.
    /// </returns>
    public double ToBaseUnit(double valueInThisUnit)
    {
      return IsBasicUnit
               ? valueInThisUnit
               : ReferencedUnit.ToBaseUnit(Coefficient * valueInThisUnit);
    }

    public double FromBaseUnit(double valueInBaseUnit)
    {
      return IsBasicUnit
               ? valueInBaseUnit
               : ReferencedUnit.FromBaseUnit(valueInBaseUnit / Coefficient);
    }

    public bool IsBasicUnit
    {
      get { return ReferencedUnit == null; }
    }

    public bool IsConvertibleTo(IUnit other)
    {
      return GetBasicUnit() == other.BasicUnit;
    }

    public IUnit BasicUnit
    {
      get { return GetBasicUnit(); }
    }

    private IUnit GetBasicUnit()
    {
      return IsBasicUnit
               ? this
               : ReferencedUnit.BasicUnit;
    }

    public override string ToString()
    {
      return Symbol;
    }
  }
}