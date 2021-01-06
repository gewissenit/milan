#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Emporer.WPF.Converter;
using NUnit.Framework;

namespace Emporer.WPF.Tests.Converter
{
  [TestFixture]
  public class TimeSpanAggregatorConverterFixture
  {
    private IEnumerable<object> GenerateRandomDataObjects(int numberOfDataObejcts, out TimeSpan targetTimeSpan)
    {
      //const long TicksOfOneYear = 31536000000;

      var converterInput = new List<object>();
      var generator = new Random();
      long tickSum = 0;
      for (var i = 0; i < numberOfDataObejcts; i++)
      {
        var randomTicks = generator.Next(0, int.MaxValue); //int Max equals approx. 25 days
        converterInput.Add(new
                           {
                             Duration = TimeSpan.FromTicks(randomTicks)
                           });
        tickSum += randomTicks;
      }
      targetTimeSpan = TimeSpan.FromTicks(tickSum);
      return converterInput;
    }

    [Test]
    public void Su_1000000_TimeSpans()
    {
      TimeSpan targetTimeSpan;
      var converterInput = GenerateRandomDataObjects(1000000, out targetTimeSpan);

      var sut = new SumOfTimeSpans();
      var result = sut.Convert(converterInput, null, "Duration", null);

      Assert.AreEqual(targetTimeSpan, result);
    }

    [Test]
    public void Su_1000_TimeSpans()
    {
      TimeSpan targetTimeSpan;
      var converterInput = GenerateRandomDataObjects(1000, out targetTimeSpan);

      var sut = new SumOfTimeSpans();
      var result = sut.Convert(converterInput, null, "Duration", null);

      Assert.AreEqual(targetTimeSpan, result);
    }

    [Test]
    public void Su_Two_TimeSpans()
    {
      var sut = new SumOfTimeSpans();
      var ts1 = new TimeSpan(1);
      var ts2 = new TimeSpan(1);
      var dataObject1 = new
                        {
                          Duration = ts1
                        };
      var dataObject2 = new
                        {
                          Duration = ts2
                        };


      var converterInput = new object[]
                           {
                             dataObject1, dataObject2
                           };

      var result = (TimeSpan) sut.Convert(converterInput, null, "Duration", null);
      Assert.AreEqual(2, result.Ticks);
    }
  }
}