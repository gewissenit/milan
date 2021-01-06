#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Emporer.WPF.Controls;

namespace Emporer.WPF.Converter
{
  [ValueConversion(typeof (bool), typeof (Thickness))]
  public class PinnedToThicknessConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var flyout = (Flyout) values[0];
      var control = (FrameworkElement) values[1];
      var oldMargin = control.Margin;
      if (flyout.Pinned)
      {
        if (flyout.HorizontalAlignment == HorizontalAlignment.Left)
        {
          return new Thickness(flyout.FlyoutWidth, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
        }
        else if (flyout.HorizontalAlignment == HorizontalAlignment.Right)
        {
          return new Thickness(oldMargin.Left, oldMargin.Top, flyout.FlyoutWidth, oldMargin.Bottom);
        }
      }
      else
      {
        if (flyout.HorizontalAlignment == HorizontalAlignment.Left)
        {
          return new Thickness(0, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
        }
        else if (flyout.HorizontalAlignment == HorizontalAlignment.Right)
        {
          return new Thickness(oldMargin.Left, oldMargin.Top, 0, oldMargin.Bottom);
        }
      }

      return new Thickness(0, 0, 0, 0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}