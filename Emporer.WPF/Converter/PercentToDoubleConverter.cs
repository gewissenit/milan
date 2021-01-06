using System;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class PercentToDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (double) value * 100;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is double) {
        return (double) value / 100;
      }
      return 0;
    }
  }
}
