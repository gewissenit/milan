using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  public abstract class StationaryElementFixture<T> : EntityFixture<T>
    where T : StationaryElement
  {
    protected IList<IEntity> _entities;
    protected MockRepository _mockRepo;

    [SetUp]
    public virtual void SetUp()
    {
      _entities = new List<IEntity>();
      _mockRepo = new MockRepository();
    }

    [TearDown]
    public virtual void TearDown()
    {
    }

    protected override void Default_Ctor(T sut)
    {
      Assert.IsEmpty(sut.Connections.ToArray());
    }

    protected Product MockProduct(IModel model, IProductType productType)
    {      
      model.Expect(m => m.GetIndexForDynamicEntity(typeof(Product)))
           .Return(20);
      model.Replay();
      var product = _mockRepo.DynamicMock<Product>(model, productType, double.NaN);
      model.BackToRecord();
      product.Expect(p => p.ProductType)
             .Return(productType)
             .Repeat.Any();
      return product;
    }

    protected Product MoqProduct(IModel model, IProductType productType)
    {      
      model.Expect(m => m.GetIndexForDynamicEntity(typeof(Product)))
           .Return(20);
      model.Replay();
      var product = _mockRepo.DynamicMock<Product>(model, productType, double.NaN);
      model.BackToRecord();
      product.Expect(p => p.ProductType)
             .Return(productType)
             .Repeat.Any();
      return product;
    }

    [Test]
    public void Add_Connection()
    {
      var sut = CreateMinimalConfiguredSUT();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();
      sut.Add(connectionMock);
      Assert.Contains(connectionMock, sut.Connections.ToArray());
      Assert.AreEqual(1, sut.Connections.Count());
    }

    [Test]
    public void Remove_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();
      sut.Add(connectionMock);
      sut.Remove(connectionMock);
      Assert.IsEmpty(sut.Connections.ToArray());
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();
      Assert.Throws<InvalidOperationException>(() => sut.Remove(connectionMock));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      IConnection connectionMock = null;
      Assert.Throws<InvalidOperationException>(() => sut.Remove(connectionMock));
    }

    [Test]
    public void Fail_On_Add_Output_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();
      sut.Add(connectionMock);
      Assert.Throws<InvalidOperationException>(() => sut.Add(connectionMock));
    }

  }
}