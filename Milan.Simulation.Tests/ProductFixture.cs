#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  internal class ProductFixture
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

    protected Product CreateSUT()
    {
      var productType = _mockRepo.DynamicMock<IProductType>();
      var model = _mockRepo.DynamicMock<IModel>();
      var product = new Product(model, productType, 0);
      model.Expect(m => m.GetIndexForDynamicEntity(typeof (Product))).Return(1);
      return product;
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateSUT();
      Assert.AreEqual(null, sut.CurrentLocation);
      Assert.IsEmpty(sut.IntegratedProducts.ToArray());
      Assert.IsNotNull(sut.ProductType);
      Assert.AreEqual(0, sut.TimeStamp);
    }

    [Test]
    public void Second_Ctor()
    {
      var model = _mockRepo.DynamicMock<IModel>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var products = new List<Product>();
      products.Add(CreateSUT());
      products.Add(CreateSUT());
      products.Add(CreateSUT());

      var sut = new Product(model, productType, 0, products);

      Assert.AreEqual(null, sut.CurrentLocation);
      Assert.IsNotEmpty(sut.IntegratedProducts.ToArray());
      Assert.IsNotNull(sut.ProductType);
      Assert.AreEqual(0, sut.TimeStamp);
    }

    [Test]
    public void Set_CurrentLocation()
    {
      var sut = CreateSUT();
      var stationaryElementStub = _mockRepo.DynamicMock<IStationaryElement>();
      var experiment = _mockRepo.DynamicMock<IExperiment>();
      stationaryElementStub.Expect(se => se.CurrentExperiment)
                           .Return(experiment);
      _mockRepo.ReplayAll();

      sut.CurrentLocation = stationaryElementStub;

      Assert.IsNotNull(sut.CurrentLocation);
    }
  }
}