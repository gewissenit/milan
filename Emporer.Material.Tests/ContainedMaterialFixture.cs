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
  public class ContainedMaterialFixture : DomainEntityFixture<ContainedMaterial>
  {
    protected override ContainedMaterial CreateSUT()
    {
      return new ContainedMaterial();
    }


    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.AreEqual(0, sut.Amount);
      Assert.AreEqual(null, sut.Material);
    }


    [Test]
    public void Set_Amount()
    {
      var value = 1;
      SetProperty("Amount", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Amount = v, s => s.Amount);
    }


    [Test]
    public void Set_Amount_Twice()
    {
      var value = 1;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Amount = v);
    }


    [Test]
    public void Set_Material()
    {
      var sut = CreateMinimalConfiguredSUT();

      var unitMock = MockRepository.GenerateStrictMock<IMaterial>();
      sut.Material = unitMock;
      Assert.AreEqual(unitMock, sut.Material);
    }
  }
}