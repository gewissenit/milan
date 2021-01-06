#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class InvertBool : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return !(bool) value;
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return !(bool) value;
      }
      return value;
    }
  }
}