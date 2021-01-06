#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows.Controls;
using Emporer.WPF.Converter;
using NUnit.Framework;
using System.Threading;
using System;
using GeWISSEN.TestUtils;

// ReSharper disable InconsistentNaming

namespace Emporer.WPF.Tests.Converter
{
  [TestFixture]
  public class HighlightTextBlockFixture
  {
    private const bool SearchCaseSensitive = true;
    private const bool SearchCaseInsensitive = false;

    [Test]
    public void By_Default_Highlighted_Text_Is_Found_Case_Insensitive()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var sut = new HighlightTextBlock();
        var text = "QWERTZUIOPÜTestqwertzuiopü";

        var searchString = "test";
        var textBlock = (TextBlock)sut.Convert(new object[]
                                                {
                                                text, searchString
                                                },
                                                null,
                                                null,
                                                null);
        Assert.AreEqual(3, textBlock.Inlines.Count);

        searchString = "Test";
        textBlock = (TextBlock)sut.Convert(new object[]
                                            {
                                            text, searchString
                                            },
                                            null,
                                            null,
                                            null);

        Assert.AreEqual(3, textBlock.Inlines.Count);
      });
    }

    [Test]
    [Apartment(ApartmentState.STA)] // the creation of WPF ui elements requires this
    public void Closing_Tags_Are_Escaped()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var sut = new HighlightTextBlock();
        var input = ">";

        var output = (TextBlock)sut.Convert(new object[]
                                             {
                                             input, string.Empty
                                             },
                                             null,
                                             null,
                                             null);

        Assert.AreEqual(input, output.Text);
      });
    }

    [Test]
    public void If_Parameter_Is_False_Highlighted_Text_Is_Found_Case_Insensitive()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var sut = new HighlightTextBlock();
        var text = "QWERTZUIOPÜTestqwertzuiopü";

        var searchString = "test";
        var textBlock = (TextBlock)sut.Convert(new object[]
                                                {
                                                text, searchString
                                                },
                                                null,
                                                SearchCaseInsensitive,
                                                null);
        Assert.AreEqual(3, textBlock.Inlines.Count);

        searchString = "Test";
        textBlock = (TextBlock)sut.Convert(new object[]
                                            {
                                            text, searchString
                                            },
                                            null,
                                            SearchCaseInsensitive,
                                            null);

        Assert.AreEqual(3, textBlock.Inlines.Count);
      });
    }

    [Test]
    public void If_Parameter_Is_True_Highlighted_Text_Is_Found_Case_Sensitive()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var sut = new HighlightTextBlock();
        var text = "QWERTZUIOPÜTestqwertzuiopü";

        var searchString = "test";
        var textBlock = (TextBlock)sut.Convert(new object[]
                                                {
                                                text, searchString
                                                },
                                                null,
                                                SearchCaseSensitive,
                                                null);
        Assert.AreEqual(1, textBlock.Inlines.Count);

        searchString = "Test";
        textBlock = (TextBlock)sut.Convert(new object[]
                                            {
                                            text, searchString
                                            },
                                            null,
                                            SearchCaseSensitive,
                                            null);

        Assert.AreEqual(3, textBlock.Inlines.Count);
      });
    }

    [Test]
    public void Opening_Tags_Are_Escaped()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {

        var input = "<";
        TextBlock output = null;

        var sut = new HighlightTextBlock();
        output = (TextBlock)sut.Convert(new object[]
                                             {
                                             input, string.Empty
                                             },
                                             null,
                                             null,
                                             null);
        Assert.AreEqual(input, output.Text);
      });   
    }
  }  
}

// ReSharper restore InconsistentNaming