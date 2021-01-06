using Emporer.WPF.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Emporer.WPF.Tests.Converter
{
  public class TimeSpanToTextFacts
  {
    public static IEnumerable<object[]> TimeSpanToText
    {
      get
      {
        return new[]
        {
          new object[]{TimeSpan.Zero, "Not set"},
          new object[]{new TimeSpan(12,9,35,7,120), "12d 9h 35m 7s 120ms"},
          new object[]{new TimeSpan(1,0,0,0,0), "1d"},
          new object[]{new TimeSpan(0,1,0,0,0), "1h"},
          new object[]{new TimeSpan(0,0,1,0,0), "1m"},
          new object[]{new TimeSpan(0,0,0,1,0), "1s"},
          new object[]{new TimeSpan(0,0,0,0,1), "1ms"},
        };
      }
    }
    public static IEnumerable<object[]> TextToTimeSpan
    {
      get
      {
        return new[]
        {
          new object[]{"12d 9h 35m 7s 120ms",new TimeSpan(12,9,35,7,120)},
          new object[]{"1d",new TimeSpan(1,0,0,0,0)},
          new object[]{"1h",new TimeSpan(0,1,0,0,0)},
          new object[]{"1m",new TimeSpan(0,0,1,0,0)},
          new object[]{"1s",new TimeSpan(0,0,0,1,0)},
          new object[]{"1ms",new TimeSpan(0,0,0,0,1)},
        };
      }
    }

    [Theory]
    [MemberData("TimeSpanToText")]
    public void It_converts_timespans_to_text(TimeSpan input, string expected)
    {
      var result = new TimeSpanToText().Convert(input, typeof(string), null, CultureInfo.InvariantCulture);
      Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData("TextToTimeSpan")]
    public void It_converts_texts_to_timespans(string input, TimeSpan expected)
    {
      var result = new TimeSpanToText().ConvertBack(input, typeof(TimeSpan), null, CultureInfo.InvariantCulture);
      Assert.Equal(expected, result);
    }

    [Fact]
    public void It_throws_ArgumentException_when_it_should_convert_an_unknown_type()
    {
      Assert.Throws<ArgumentException>(() => new TimeSpanToText().Convert(new object(), typeof(string), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void It_throws_ArgumentException_when_it_should_convert_back_an_unknown_type()
    {
      Assert.Throws<ArgumentException>(() => new TimeSpanToText().ConvertBack(new object(), typeof(TimeSpan), null, CultureInfo.InvariantCulture));
    }
  }
}