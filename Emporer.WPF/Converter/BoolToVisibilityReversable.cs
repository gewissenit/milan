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
  public class BoolToVisibilityReversable : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((string) parameter == "true")
      {
        return (bool) value
                 ? Visibility.Visible
                 : Visibility.Collapsed;
      }
      else
      {
        return (bool) value
                 ? Visibility.Collapsed
                 : Visibility.Visible;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility)
      {
        var visibility = (Visibility) value;
        return (string) parameter == "true"
                 ? visibility == Visibility.Visible
                 : visibility == Visibility.Collapsed;
      }
      else
      {
        return false;
      }
    }
  }
}