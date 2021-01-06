#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Milan.Simulation.UI.Converters
{
  [ValueConversion(typeof (double), typeof (DateTime))]
  public class SimulationTimeToRealTimeConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var simulationTime = (double) values[0];
      if (values[1] == null)
      {
        return simulationTime;
      }
      var startDate = (DateTime) values[1];
      var date = simulationTime.ToRealDate(startDate);
      if (targetType == typeof (string))
      {
        return date.ToString();
      }
      return date;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  [ValueConversion(typeof(double), typeof(TimeSpan))]
  public class SimulationTicksToTimeSpanConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is long ||
          value is double ||
          value is int ||
          value is Single ||
          value is byte)
      {
        return System.Convert.ToDouble(value)
                     .ToRealTimeSpan();
      }
      return null;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((TimeSpan) value).ToSimulationTimeSpan();
    }
  }
}