using System;

namespace Milan.VisualModeling.Utilities
{
  public static class ValueExtensions
  {
    public static void ChangeButKeepAtLeastZero(Func<double> getCurrentValue, Action<double> setValue, double value)
    {
      setValue(getCurrentValue() < 0
                 ? 0
                 : value);
    }
  }
}