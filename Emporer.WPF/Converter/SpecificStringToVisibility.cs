using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class SpecificStringToVisibility: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var text = value as string;
      var criterion = parameter as string;

      if (text == null || criterion == null) return Visibility.Visible;

      return text == criterion
               ? Visibility.Collapsed
               : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}