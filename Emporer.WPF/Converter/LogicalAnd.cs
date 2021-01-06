#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class LogicalAnd : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Any(v => !(v is bool))) // all inputs have to be bool
      {
        return Binding.DoNothing;
      }
      var bools = values.Cast<bool>();

      var result = bools.First();

      result = bools.Skip(1)
                    .Aggregate(result, (current, next) => current & next); // logial AND all bools with the first one
      return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}