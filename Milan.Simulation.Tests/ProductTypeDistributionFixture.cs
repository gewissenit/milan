#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Math.Distribution;
using Milan.JsonStore.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class ProductTypeDistributionFixture : DomainEntityFixture<ProductTypeDistribution>
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

    protected override ProductTypeDistribution CreateSUT()
    {
      return new ProductTypeDistribution();
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.AreEqual(null, sut.DistributionConfiguration);
      Assert.AreEqual(null, sut.ProductType);
    }

    [Test]
    public void Set_DistributionConfiguration()
    {
      var value = _mockRepo.DynamicMock<DistributionConfiguration>();
      SetProperty("DistributionConfiguration",
                  value,
                  (s, v) => Assert.IsTrue(s == v),
                  (s, v) => s.DistributionConfiguration = v,
                  s => s.DistributionConfiguration);
    }


    [Test]
    public void Set_ProductType()
    {
      var value = _mockRepo.DynamicMock<IProductType>();
      SetProperty("ProductType", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.ProductType = v, s => s.ProductType);
    }

    [Test]
    public void Set_Same_DistributionConfiguration_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.DynamicMock<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.DistributionConfiguration = v);
    }


    [Test]
    public void Set_Same_ProductType_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.DynamicMock<IProductType>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.ProductType = v);
    }
  }
}