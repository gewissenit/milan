#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class HasItems : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return Binding.DoNothing;
      }


      if ((value is IEnumerable))
      {
        var list = (IEnumerable) value;
        return list.OfType<object>()
                   .Any();
      }

      if ((value is int))
      {
        var count = (int) value;
        return count > 0;
      }

      return Binding.DoNothing;
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}