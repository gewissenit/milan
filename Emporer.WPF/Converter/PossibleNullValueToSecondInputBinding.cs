#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class PossibleNullValueToSecondInputBinding : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2)
      {
        return Binding.DoNothing;
      }

      if (values[0] != null)
      {
        return values[0];
      }
      return values[1];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      if (value is NullValue)
      {
        return new object[]
               {
                 null
               };
      }
      return new[]
             {
               value
             };
    }
  }
}