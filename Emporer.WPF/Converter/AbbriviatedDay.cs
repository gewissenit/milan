#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class AbbreviatedDay : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var day = (DayOfWeek) value;
      return GetAbbreviatedDay(day);
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private static string GetAbbreviatedDay(DayOfWeek day)
    {
      switch (day)
      {
        case DayOfWeek.Monday:
          return "MO";
        case DayOfWeek.Tuesday:
          return "TU";
        case DayOfWeek.Wednesday:
          return "WE";
        case DayOfWeek.Thursday:
          return "TH";
        case DayOfWeek.Friday:
          return "FR";
        case DayOfWeek.Saturday:
          return "SA";
        case DayOfWeek.Sunday:
          return "SU";
        default:
          throw new InvalidOperationException();
      }
    }
  }
}