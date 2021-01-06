#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  /// <summary>
  ///   Abstract base class for converters that aggregate a list of values.
  ///   In its <see cref="Convert" /> method, a list of numeric values (short
  /// </summary>
  public abstract class AggregateOfTimeSpans : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }

      if (parameter == null)
      {
        return null;
      }

      if (!(value is IEnumerable))
      {
        return null;
      }


      var list = (IEnumerable) value;

      ICollection<TimeSpan> values = new List<TimeSpan>();

      foreach (var item in list)
      {
        if (item is CollectionViewGroup)
        {
          var cvg = (CollectionViewGroup) item;

          var subGroupValue = Convert(cvg.Items, targetType, parameter, culture);
          if (subGroupValue != null &&
              subGroupValue is TimeSpan)
          {
            values.Add((TimeSpan) subGroupValue);
          }
        }
        else
        {
          var property = item.GetType()
                             .GetProperty((string) parameter);
          var unknownPropertyValue = property.GetValue(item, null);
          if (unknownPropertyValue is TimeSpan)
          {
            values.Add((TimeSpan) unknownPropertyValue);
          }
        }
      }

      return Aggregate(values);
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public abstract TimeSpan Aggregate(IEnumerable<TimeSpan> values);
  }
}