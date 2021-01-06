using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Milan.UI
{
  [ValueConversion(typeof(object),typeof(double))]
  public class PlacePopupOnRightCenterOfTarget : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return 0;
      }
      var child = (FrameworkElement) value;
      child.Measure(new Size(1000, 1000));
      var v = child.DesiredSize.Width / 2 + 43/2;
      return v;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}