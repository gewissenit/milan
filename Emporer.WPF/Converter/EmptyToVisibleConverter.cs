#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class EmptyToVisibleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return Visibility.Visible;
      }

      if ((value is IEnumerable))
      {
        var list = (IEnumerable) value;
        return list.OfType<object>()
                   .Any()
               ? Visibility.Collapsed
               : Visibility.Visible;
      }

      if ((value is int))
      {
        var count = (int) value;
        return count > 0;
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}