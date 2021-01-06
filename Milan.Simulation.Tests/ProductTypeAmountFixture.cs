#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  internal class ProductTypeAmountFixture : DomainEntityFixture<ProductTypeAmount>
  {
    [SetUp]
    public void SetUp()
    {
      _mockRepo = new MockRepository();
    }

    [TearDown]
    public void TearDown()
    {
    }

    private MockRepository _mockRepo;

    protected override ProductTypeAmount CreateSUT()
    {
      return new ProductTypeAmount();
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateSUT();
      Assert.AreEqual(sut.Amount, 1);
      Assert.IsNull(sut.ProductType);
    }

    [Test]
    public void Second_Ctor()
    {
      var productType = _mockRepo.DynamicMock<IProductType>();
      var sut = new ProductTypeAmount(productType, int.MaxValue);

      Assert.AreEqual(sut.Amount, int.MaxValue);
      Assert.AreEqual(sut.ProductType, productType);
    }

    [Test]
    public void Set_Amount()
    {
      var value = 5;
      SetProperty("Amount", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Amount = v, s => s.Amount);
    }


    [Test]
    public void Set_ProductType()
    {
      var value = _mockRepo.DynamicMock<IProductType>();
      SetProperty("ProductType", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.ProductType = v, s => s.ProductType);
    }

    [Test]
    public void Set_Same_Amount_Does_Not_Raise_PropertyChanged()
    {
      var value = 4;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Amount = v);
    }


    [Test]
    public void Set_Same_ProductType_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.DynamicMock<IProductType>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.ProductType = v);
    }
  }
}