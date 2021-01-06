#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Emporer.Material.Tests
{
  [TestFixture]
  public class PropertyFixture : DomainEntityFixture<MaterialProperty>
  {
    protected override MaterialProperty CreateSUT()
    {
      return new MaterialProperty();
    }

    //TODO: use local variable for numbers for comparisons (like check)
    [Test]
    public void Set_Mean()
    {
      var sut = CreateMinimalConfiguredSUT();
      sut.Mean = 10.05;
      Assert.AreEqual(10.05, sut.Mean);
    }

    [Test]
    public void Set_PropertyType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var check = MockRepository.GenerateMock<IPropertyType>();
      sut.PropertyType = check;
      Assert.AreEqual(check, sut.PropertyType);
    }
  }
}