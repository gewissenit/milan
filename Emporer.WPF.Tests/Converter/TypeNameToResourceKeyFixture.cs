#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows;
using Emporer.WPF.Converter;
using NUnit.Framework;
using GeWISSEN.TestUtils;
using System.Threading;

namespace Emporer.WPF.Tests.Converter
{
  // ReSharper disable InconsistentNaming

  [TestFixture]
  public class TypeNameToResourceKeyFixture
  {
    [SetUp]
    public void Setup()
    {
      TypeNameToResource.ClearCache();
    }

    

    [Test]
    public void Returns_Match_For_Interface_If_FrameworkElement_Has_Matching_Resource_Key()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var source = new A();
        const string resourceKey = "Z"; // should match inherited interface 'IZ'
        var targetResource = new object();

        var fwElement = new FrameworkElement();
        fwElement.Resources.Add(resourceKey, targetResource);

        var value = new object[]
                    {
                    source, fwElement
                    };


        var result = new TypeNameToResource().Convert(value, null, null, null);
        Assert.AreEqual(targetResource, result);
      });
    }

    [Test]
    public void Returns_Match_If_FrameworkElement_Has_Matching_Resource_Key()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var source = new A();
        var targetResource = new object();

        var fwElement = new FrameworkElement();
        fwElement.Resources.Add("A", targetResource);

        var value = new object[]
                    {
                    source, fwElement
                    };

        var result = new TypeNameToResource().Convert(value, null, null, null);
        Assert.AreEqual(targetResource, result);
      });
    }

    [Test]
    public void Returns_Match_If_FrameworkElement_Has_Matching_Resource_Key_For_Base_Class()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var source = new A();
        var targetResource = new object();

        var fwElement = new FrameworkElement();
        fwElement.Resources.Add("B", targetResource);

        var value = new object[]
                    {
                    source, fwElement
                    };

        var result = new TypeNameToResource().Convert(value, null, null, null);
        Assert.AreEqual(targetResource, result);
      });
    }


    [Test]
    public void Returns_Match_If_FrameworkElement_Has_Matching_Resource_Key_Using_Format_Parameter()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var source = new A();
        const string parameter = "{0}Icon";
        const string resourceKey = "AIcon";
        var targetResource = new object();

        var fwElement = new FrameworkElement();
        fwElement.Resources.Add(resourceKey, targetResource);

        var value = new object[]
                    {
                    source, fwElement
                    };


        var result = new TypeNameToResource().Convert(value, null, parameter, null);
        Assert.AreEqual(targetResource, result);
      });
    }

    [Test]
    public void Returns_UnsetValue_If_No_MatchingResource_Exists()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        var source = new A();
        var fwElement = new FrameworkElement();

        var value = new object[]
                    {
                    source, fwElement
                    };


        var result = new TypeNameToResource().Convert(value, null, null, null);
        Assert.AreEqual(DependencyProperty.UnsetValue, result);
      });
    }

    [Test]
    public void Returns_UnsetValue_If_Second_Argument_Is_Not_A_FrameworkElement()
    {
      var value = new[]
                  {
                    new object(), new object()
                  };


      var result = new TypeNameToResource().Convert(value, null, null, null);
      Assert.AreEqual(DependencyProperty.UnsetValue, result);
    }

    [Test]
    public void Returns_UnsetValue_On_First_Value_Is_Null()
    {
      var value = new[]
                  {
                    null, new object()
                  };
      var result = new TypeNameToResource().Convert(value, null, null, null);
      Assert.AreEqual(DependencyProperty.UnsetValue, result);
    }

    [Test]
    public void Returns_UnsetValue_On_Missing_Value()
    {
      var value = new[]
                  {
                    new object()
                  };

      var result = new TypeNameToResource().Convert(value, null, null, null);
      Assert.AreEqual(DependencyProperty.UnsetValue, result);
    }

    [Test]
    public void Returns_UnsetValue_On_Null_Value_Argument()
    {
      var result = new TypeNameToResource().Convert(null, null, null, null);
      Assert.AreEqual(DependencyProperty.UnsetValue, result);
    }

    [Test]
    public void Throws_NotImplementedException_OnConvertBack()
    {
      Assert.Throws<NotImplementedException>(()=>new TypeNameToResource().ConvertBack(null, null, null, null)); // for 100% coverage
    }
  }

  // ReSharper restore InconsistentNaming
}