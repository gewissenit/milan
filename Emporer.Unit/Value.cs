#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Unit
{
  public class Value : IValue
  {
    private Value()
    {
      Amount = 0;
    }

    public Value(IUnit unit)
      : this()
    {
      Unit = unit;
    }

    public Value(double amount, IUnit unit)
    {
      Amount = amount;
      Unit = unit;
    }

    public double Amount { get; set; }
    public IUnit Unit { get; private set; }

    public override string ToString()
    {
      var unit = string.Empty;
      if (Unit != null &&
          Unit.Symbol != null)
      {
        unit = Unit.Symbol;
      }

      return string.Format("{0:0.00} {1}", Amount, unit);
    }
  }
}