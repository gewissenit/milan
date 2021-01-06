#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;
using NUnit.Framework;

namespace Emporer.WPF.Tests.Converter
{
  public abstract class ConverterFixtureBase<TSUT>
    where TSUT : IValueConverter
  {
    protected abstract TSUT CreateSUT();

    protected void CheckConversion(object item, object parameter, object expectedResult)
    {
      CheckConversion(item, null, parameter, null, expectedResult);
    }

    protected void CheckParameterlessConversion(object item, object expectedResult)
    {
      CheckConversion(item, null, null, null, expectedResult);
    }

    protected void CheckConversion(object item, Type targetType, object parameter, CultureInfo culture, object expectedResult)
    {
      var sut = CreateSUT();
      var conversionResult = sut.Convert(item, targetType, parameter, culture);
      Assert.AreEqual(expectedResult, conversionResult);
    }
  }
}