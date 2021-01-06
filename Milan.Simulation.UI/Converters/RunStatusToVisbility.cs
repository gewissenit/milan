#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Milan.Simulation.UI.Converters
{
  public sealed class RunStatusToVisibility : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is RunStatus))
      {
        return DependencyProperty.UnsetValue;
      }

      var runStatus = (RunStatus) value;

      var visibleForValuesParameter = parameter as string;
      if (visibleForValuesParameter == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var visibleForValues = visibleForValuesParameter.Split(',')
                                                      .Select(Parse)
                                                      .Where(x => x.HasValue);

      return visibleForValues.Contains(runStatus)
               ? Visibility.Visible
               : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("IsExecutableToVisbility can only be used for one way conversion.");
    }

    private RunStatus? Parse(string value)
    {
      RunStatus runStatus;
      if (Enum.TryParse(value, true, out runStatus))
      {
        return runStatus;
      }
      return null;
    }
  }
}