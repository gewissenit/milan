#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore.Tests;
using NUnit.Framework;

namespace Emporer.Material.Tests
{
  [TestFixture]
  public class PropertyTypeFixture : DomainEntityFixture<PropertyType>
  {
    protected override PropertyType CreateSUT()
    {
      return new PropertyType();
    }

    [Test]
    public void Set_EcoCat()
    {
      var sut = CreateMinimalConfiguredSUT();
      var check = "Kategorie A";
      sut.EcoCat = check;
      Assert.AreEqual(check, sut.EcoCat);
    }

    [Test]
    public void Set_EcoSubCat()
    {
      var sut = CreateMinimalConfiguredSUT();
      var check = "Kategorie A -> B";
      sut.EcoSubCat = check;
      Assert.AreEqual(check, sut.EcoSubCat);
    }

    [Test]
    public void Set_Location()
    {
      var sut = CreateMinimalConfiguredSUT();
      var check = "Gebäude Test";
      sut.Location = check;
      Assert.AreEqual(check, sut.Location);
    }

    [Test]
    public void Set_Name()
    {
      var sut = CreateMinimalConfiguredSUT();
      var check = "sut";
      sut.Name = check;
      Assert.AreEqual(check, sut.Name);
    }

    [Test]
    public void Set_Unit()
    {
      var sut = CreateMinimalConfiguredSUT();
      var check = "Unit Test";
      sut.Unit = check;
      Assert.AreEqual(check, sut.Unit);
    }
  }
}