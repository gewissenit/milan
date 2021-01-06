using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class Multiply : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values.Select(System.Convert.ToDouble)
                   .Aggregate(1.0, (c, t) => c * t);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new Exception("Not implemented");
    }
  }
}