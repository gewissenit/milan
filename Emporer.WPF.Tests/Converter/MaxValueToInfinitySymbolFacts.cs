using Emporer.WPF.Converter;
using NUnit.Framework;
using System;

namespace Emporer.WPF.Tests.Converter
{
  [TestFixture]
  public class MaxValueToInfinitySymbolFacts
  {
    private const string InfinitySymbol = "∞";
    private string _output;

    [Test]
    [TestCase(byte.MaxValue)]
    [TestCase(short.MaxValue)]
    [TestCase(int.MaxValue)]
    [TestCase(long.MaxValue)]
    [TestCase(float.MaxValue)]
    [TestCase(double.MaxValue)]
    public void It_converts_some_maximum_to_the_infinity_symbol(object input)
    {
      _output = (string)new MaxValueToInfinitySymbol().Convert(input, null, null, null);
      Assert.AreEqual(InfinitySymbol, _output);
    }

    public void It_converts_decimal_maximum_to_the_infinity_symbol()
    {
      _output = (string)new MaxValueToInfinitySymbol().Convert(decimal.MaxValue, null, null, null);
      Assert.AreEqual(InfinitySymbol, _output);
    }

    [Test]
    [TestCase(new object[] { byte.MaxValue, typeof(byte) })]
    [TestCase(new object[] { short.MaxValue, typeof(short) })]
    [TestCase(new object[] { int.MaxValue, typeof(int) })]
    [TestCase(new object[] { long.MaxValue, typeof(long) })]
    [TestCase(new object[] { float.MaxValue, typeof(float) })]
    [TestCase(new object[] { double.MaxValue, typeof(double) })]
    public void It_converts_the_infinity_symbol_back_to_the_maximum_of_the_requested_data_type(object expected, Type targetType)
    {
      Assert.AreEqual(expected, new MaxValueToInfinitySymbol().ConvertBack(InfinitySymbol, targetType, null, null));
    }
  }
}
