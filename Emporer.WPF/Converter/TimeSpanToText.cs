using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  public class TimeSpanToText : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is TimeSpan))
      {
        throw new ArgumentException("Not a timespan.");
      }
      var input = (TimeSpan)value;

      if (input == TimeSpan.Zero)
      {
        return "Not set";
      }

      // cut away leading or trailing empty components (0 days, 0 milliseconds, ...)
      var d0 = input.Days == 0;
      var h0 = input.Hours == 0;
      var m0 = input.Minutes == 0;
      var s0 = input.Seconds == 0;
      var i0 = input.Milliseconds == 0;

      var d = d0 ? "" : $"{input.Days}d";
      var h = ((d0 && h0) || (i0 && s0 && m0 && h0)) ? "" : $"{input.Hours}h";
      var m = ((d0 && h0 && m0) || (i0 && s0 && m0)) ? "" : $"{input.Minutes}m";
      var s = ((d0 && h0 && m0 && s0) || (i0 && s0)) ? "" : $"{input.Seconds}s";
      var i = i0 ? "" : $"{input.Milliseconds}ms";

      return new[] { d, h, m, s, i }.Where(x => x != "").Aggregate((c, n) => $"{c} {n}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is string))
      {
        throw new ArgumentException("Not a timespan.");
      }

      var input = (string)value;
      var components=input.Split(' ').Select(i =>
      {
        var component = GetComponent(i);
        return new
        {
          C = component,
          V = ParseComponent(i, component)
        };
      }).ToDictionary(i => i.C, i => i.V);

      return new TimeSpan(components.ContainsKey("d") ? components["d"] : 0, components.ContainsKey("h")?components["h"]:0, components.ContainsKey("m") ? components["m"] : 0, components.ContainsKey("s") ? components["s"] : 0, components.ContainsKey("ms") ? components["ms"] : 0);
    }

    private string GetComponent(string input)
    {
      return new[] { "d", "h", "m", "ms", "s" }.First(input.EndsWith);
    }

    private int ParseComponent(string input, string component)
    {
      return input.EndsWith(component)
        ? int.Parse(input.Substring(0, input.Length - component.Length))
        : 0;
    }
  }
}