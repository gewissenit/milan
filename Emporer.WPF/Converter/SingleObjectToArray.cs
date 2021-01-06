#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class SingleObjectToArray : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new[]
             {
               value
             };
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var array = value as IEnumerable;
      if (array == null)
      {
        return Binding.DoNothing;
      }

      var enumerator = array.GetEnumerator();
      return enumerator.MoveNext()
               ? enumerator.Current
               : Binding.DoNothing;
    }
  }
}