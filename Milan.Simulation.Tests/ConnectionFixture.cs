#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Milan.JsonStore.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  internal class ConnectionFixture : DomainEntityFixture<Connection>
  {
    [SetUp]
    public virtual void SetUp()
    {
      _mockRepo = new MockRepository();
    }

    protected MockRepository _mockRepo;

    protected override Connection CreateSUT()
    {
      return new Connection();
    }

    [Test]
    public void Add_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductType>();
      sut.Add(item);
      Assert.Contains(item, sut.ProductTypes.ToArray());
      Assert.AreEqual(1, sut.ProductTypes.Count());
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.IsNull(sut.Destination);
      Assert.IsFalse(sut.IsRoutingPerProductType);
      Assert.IsEmpty(sut.ProductTypes.ToArray());
      Assert.AreEqual(0, sut.Priority);
    }

    [Test]
    public void Fail_On_Add_ProductType_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductType>();
      sut.Add(item);
      Assert.Throws<InvalidOperationException>(()=>sut.Add(item));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductType>();
      Assert.Throws<InvalidOperationException>(() => sut.Remove(item));
    }

    [Test]
    public void Fail_On_Remove_Null_From_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      IProductType item = null;
      Assert.Throws<InvalidOperationException>(() => sut.Remove(item));
    }

    [Test]
    public void Remove_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductType>();
      sut.Add(item);
      sut.Remove(item);
      Assert.IsEmpty(sut.ProductTypes.ToArray());
    }

    [Test]
    public void Set_Destination()
    {
      var value = _mockRepo.Stub<IStationaryElement>();
      SetProperty("Destination", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Destination = v, s => s.Destination);
    }

    [Test]
    public void Set_IsRoutingPerProductType()
    {
      var value = true;
      SetProperty("IsRoutingPerProductType",
                  value,
                  (s, v) => Assert.IsTrue(s == v),
                  (s, v) => s.IsRoutingPerProductType = v,
                  s => s.IsRoutingPerProductType);
    }

    [Test]
    public void Set_Priority()
    {
      var value = 2;
      SetProperty("Priority", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Priority = v, s => s.Priority);
    }

    [Test]
    public void Set_Same_Destination_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.Stub<IStationaryElement>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Destination = v);
    }

    [Test]
    public void Set_Same_IsRoutingPerProductType_Does_Not_Raise_PropertyChanged()
    {
      var value = true;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.IsRoutingPerProductType = v);
    }

    [Test]
    public void Set_Same_Priority_Does_Not_Raise_PropertyChanged()
    {
      var value = 4;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Priority = v);
    }
  }
}