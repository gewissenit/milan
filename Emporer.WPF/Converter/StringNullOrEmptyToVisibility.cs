#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class StringNullOrEmptyToVisibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is string))
      {
        return null;
      }

      var hide = false;

      if (parameter is string)
      {
        hide = ((string) parameter) == "hide";
      }

      if (string.IsNullOrEmpty((string) value))
      {
        return hide
                 ? Visibility.Hidden
                 : Visibility.Collapsed;
      }

      return Visibility.Visible;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}