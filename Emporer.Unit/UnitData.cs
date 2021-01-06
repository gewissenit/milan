using FileHelpers;

namespace Emporer.Unit
{
  [DelimitedRecord(";")]
// ReSharper disable ClassNeverInstantiated.Global
  internal class UnitData
// ReSharper restore ClassNeverInstantiated.Global
  {
    public UnitData()
    {
      id = string.Empty;
      dimension = string.Empty;
      name = string.Empty;
      symbol = string.Empty;
      referencedUnit = string.Empty;
      coefficient = 0.0;
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    // ReSharper disable UnassignedField.Global
// ReSharper disable UnassignedField.Compiler
    public string id;
    public string dimension;
    public string name;
    public string symbol;
    public string referencedUnit;
    public double? coefficient;
// ReSharper restore UnassignedField.Compiler
    // ReSharper restore UnassignedField.Global
    // ReSharper restore FieldCanBeMadeReadOnly.Global
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore InconsistentNaming
  }
}