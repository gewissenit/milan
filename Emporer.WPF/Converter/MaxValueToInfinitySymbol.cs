using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class MaxValueToInfinitySymbol : IValueConverter
  {

    const string InfinitySymbol = "∞";


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is byte)
      {
        return InfinityOrValue((byte)value);
      }

      if (value is short)
      {
        return InfinityOrValue((short)value);
      }

      if (value is int)
      {
        return InfinityOrValue((int)value);
      }

      if (value is long)
      {
        return InfinityOrValue((long)value);
      }

      if (value is decimal)
      {
        return InfinityOrValue((decimal)value);
      }

      if (value is float)
      {
        return InfinityOrValue((float)value);
      }

      if (value is double)
      {
        return InfinityOrValue((double)value);
      }

      if (value is decimal)
      {
        return InfinityOrValue((decimal)value);
      }

      return value;
    }

    private object InfinityOrValue(byte value)
    {
      return value == byte.MaxValue ? InfinitySymbol : (object)value;
    }

    private object InfinityOrValue(short value)
    {
      return value == short.MaxValue ? InfinitySymbol : (object)value;
    }
    private object InfinityOrValue(int value)
    {
      return value == int.MaxValue ? InfinitySymbol : (object)value;
    }
    private object InfinityOrValue(long value)
    {
      return value == long.MaxValue ? InfinitySymbol : (object)value;
    }
    private object InfinityOrValue(float value)
    {
      return value == float.MaxValue ? InfinitySymbol : (object)value;
    }
    private object InfinityOrValue(double value)
    {
      return value == double.MaxValue ? InfinitySymbol : (object)value;
    }
    private object InfinityOrValue(decimal value)
    {
      return value == decimal.MaxValue ? InfinitySymbol : (object)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var text = value as string;
      return InfinitySymbol == text ? _maxValues[targetType] : value;
    }

    private static readonly Dictionary<Type, object> _maxValues;

    static  MaxValueToInfinitySymbol()
    {
      _maxValues = new Dictionary<Type, object>()
      {
        {typeof(byte),byte.MaxValue },
        {typeof(short),short.MaxValue },
        {typeof(int),int.MaxValue },
        {typeof(long),long.MaxValue },
        {typeof(float),float.MaxValue },
        {typeof(double),double.MaxValue },
        {typeof(decimal),decimal.MaxValue },
      };
      
    }
  }
}
