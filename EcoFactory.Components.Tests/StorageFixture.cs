#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public class StorageFixture : StationaryElementFixture<Storage>
  {
    private Storage _sut;
    private SpyScheduler _Scheduler;

    protected override void Default_Ctor(Storage sut)
    {
      Assert.AreEqual(1, sut.Capacity);
      Assert.IsFalse(sut.HasLimitedCapacity);
      Assert.IsFalse(sut.HasCapacityPerProductType);
      Assert.IsEmpty(sut.ProductTypeCapacities.ToArray());
      Assert.IsEmpty(sut.ProductTypeCounts.ToArray());
    }

    protected override Storage CreateSUT()
    {
      return new Storage();
    }

    private void AssertScheduledItemsWithProducts(IEnumerable<Product> products)
    {
      foreach (var product in products)
      {
        Assert.True(_Scheduler.OfType<IProductsRelatedEvent>()
                               .Any(i => i.Products.Contains(product)));
        _Scheduler.ProcessNextSchedulable();
        Assert.True(_Scheduler.ProcessedItems.OfType<IProductsRelatedEvent>()
                               .Any(i => i.Products.Contains(product)));
      }
    }

    private void AddProductTypeCapacities(IEnumerable<IProductTypeAmount> productTypeCapacities)
    {
      foreach (var productTypeCapacity in productTypeCapacities)
      {
        _sut.AddProductTypeCapacity(productTypeCapacity);
      }
    }

    private IEnumerable<IProductTypeAmount> CreateProductTypeCapacities(IEnumerable<IProductType> productTypes)
    {
      var list = new List<IProductTypeAmount>();
      foreach (var productType in productTypes)
      {
        var productTypeAmount = _mockRepo.DynamicMock<IProductTypeAmount>();
        productTypeAmount.Expect(p => p.ProductType)
                         .Return(productType);
        productTypeAmount.Expect(p => p.Amount)
                         .Return(1);
        list.Add(productTypeAmount);
      }
      return list;
    }

    private void ReceiveProducts(IEnumerable<Product> products)
    {
      foreach (var product in products)
      {
        _sut.Receive(product);
      }
    }

    private Product[] CreateProducts(IEnumerable<IProductType> productTypes, IModel model)
    {
      return productTypes.Select(pt => MockProduct(model, pt))
                         .ToArray();
    }

    private IProductType[] CreateProductTypes(int amount)
    {
      var list = new List<IProductType>();
      for (var i = 0; i < amount; i++)
      {
        var pt = _mockRepo.DynamicMock<IProductType>();
        list.Add(pt);
        _entities.Add(pt);
      }
      return list.ToArray();
    }

    private void CreateMinimalConfiguration()
    {
      _mockRepo = new MockRepository();
      _sut = new Storage
              {
                Model = _mockRepo.DynamicMock<IModel>(),
                HasLimitedCapacity = true
              };
    }

    private void SetExperimentAndScheduler()
    {
      var currentExperiment = MockRepository.GenerateMock<IExperiment>();
      _Scheduler = new SpyScheduler();
      currentExperiment.Expect(x => x.Scheduler)
                       .Return(_Scheduler);
      _sut.CurrentExperiment = currentExperiment;
    }

    [Test]
    public void Add_ProductTypeCapacity()
    {
      _sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeAmount>();
      _sut.AddProductTypeCapacity(item);
      Assert.Contains(item, _sut.ProductTypeCapacities.ToArray());
      Assert.AreEqual(1, _sut.ProductTypeCapacities.Count());
    }

    [Test]
    public void Fail_On_Add_ProductTypeCapacity_Twice()
    {
      _sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeAmount>();
      _sut.AddProductTypeCapacity(item);
      Assert.Throws<InvalidOperationException>(() => _sut.AddProductTypeCapacity(item));
    }

    [Test]
    public void Fail_On_Add_Second_ProductTypeCapacity_With_Same_ProductType()
    {
      _sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeAmount>();
      var item2 = _mockRepo.DynamicMock<IProductTypeAmount>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      item.Expect(i => i.ProductType)
          .Return(productTypeMock);
      item2.Expect(i => i.ProductType)
           .Return(productTypeMock);
      _sut.AddProductTypeCapacity(item);
      _sut.AddProductTypeCapacity(item2);
      Assert.Throws<InvalidOperationException>(() => _mockRepo.VerifyAll());
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_ProductTypeCapacity()
    {
      _sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeAmount>();
      Assert.Throws<InvalidOperationException>(() => _sut.RemoveProductTypeCapacity(item));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_ProductTypeCapacity()
    {
      _sut = CreateMinimalConfiguredSUT();
      Assert.Throws<InvalidOperationException>(() => _sut.RemoveProductTypeCapacity(null));
    }

    [Test]
    public void Failed_If_Not_Asking_If_Full_With_Setting_A_Common_Capacity()
    {
      CreateMinimalConfiguration();
      SetExperimentAndScheduler();
      _sut.Capacity = 1;
      var productTypes = CreateProductTypes(2);
      var products = CreateProducts(productTypes.Take(_sut.Capacity), _sut.Model);
      var p = MockProduct(_sut.Model, productTypes.Last());
      _mockRepo.ReplayAll();

      _sut.Initialize();
      ReceiveProducts(products);
      AssertScheduledItemsWithProducts(products);
      Assert.False(_sut.IsAvailable(p));
      Assert.Throws<InvalidOperationException>(() => _sut.Receive(p));
    }

    [Test]
    public void Failed_If_Not_Asking_If_Full_With_Setting_ProductTypeCapacities()
    {
      CreateMinimalConfiguration();
      SetExperimentAndScheduler();
      _sut.HasCapacityPerProductType = true;
      var productTypes = CreateProductTypes(1);
      var products = CreateProducts(productTypes, _sut.Model);
      var product = CreateProducts(productTypes, _sut.Model)
        .Single();
      var productTypeCapacities = CreateProductTypeCapacities(productTypes);
      _mockRepo.ReplayAll();
      AddProductTypeCapacities(productTypeCapacities);

      _sut.Initialize();
      ReceiveProducts(products);
      AssertScheduledItemsWithProducts(products);
      Assert.False(_sut.IsAvailable(product));
      Assert.Throws<InvalidOperationException>(() => _sut.Receive(product));
    }

    [Test]
    public void Failed_If_Setting_Limited_Capacity_Without_Setting_Capacity_Or_Adding_A_ProductTypeCapacity()
    {
      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      _sut = CreateMinimalConfiguredSUT();
      _sut.CurrentExperiment = experimentMock;
      _sut.Model = modelMock;
      _sut.HasLimitedCapacity = true;
      _sut.Capacity = 0;

      Assert.Throws<ModelConfigurationException>(() => _sut.Initialize());
    }

    [Test]
    public void Failed_If_Setting_ProductType_Depending_Capacity_Without_Adding_A_ProductTypeCapacity()
    {
      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      _sut = CreateMinimalConfiguredSUT();
      _sut.CurrentExperiment = experimentMock;
      _sut.Model = modelMock;
      _sut.HasLimitedCapacity = true;
      _sut.HasCapacityPerProductType = true;

      Assert.Throws<ModelConfigurationException>(() => _sut.Initialize());
    }

    [Test]
    public void Gets_Full_If_Cannot_Send()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var connection = _mockRepo.DynamicMock<IConnection>();
      var successor = _mockRepo.DynamicMock<IStationaryElement>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);
      var product2 = MockProduct(modelMock, productType);
      var product3 = MockProduct(modelMock, productType);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connection.Expect(c => c.Destination)
                .Return(successor);
      connection.Expect(c => c.ProductTypes)
                .Return(new IProductType[0]);
      successor.Expect(se => se.IsAvailable(product1))
               .Return(false);

      _mockRepo.ReplayAll();

      _entities.Add(successor);

      _sut = CreateMinimalConfiguredSUT();
      _sut.HasLimitedCapacity = true;
      _sut.Model = modelMock;
      _sut.CurrentExperiment = experimentMock;
      _sut.Capacity = 2;
      _sut.Add(connection);

      _sut.Initialize();
      _sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      _sut.Receive(product2);
      scheduler.ProcessNextSchedulable();
      Assert.False(_sut.IsAvailable(product3));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Available_For_Another_ProductType_If_One_ProductType_Is_Full()
    {
      CreateMinimalConfiguration();
      SetExperimentAndScheduler();
      _sut.HasCapacityPerProductType = true;
      var productTypes = CreateProductTypes(3);
      var products = CreateProducts(productTypes.Take(2), _sut.Model);
      var productTypeCapacities = CreateProductTypeCapacities(productTypes);
      var products2 = CreateProducts(productTypes, _sut.Model);
      _mockRepo.ReplayAll();
      AddProductTypeCapacities(productTypeCapacities);

      _sut.Initialize();
      ReceiveProducts(products);
      AssertScheduledItemsWithProducts(products);
      Assert.True(_sut.IsAvailable(products2.Last()));
      Assert.False(_sut.IsAvailable(products2.First()));
      Assert.False(_sut.IsAvailable(products2.Skip(1)
                                              .First()));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Not_Available_If_Full_With_Setting_A_Common_Capacity()
    {
      CreateMinimalConfiguration();
      SetExperimentAndScheduler();
      _sut.Capacity = 5;
      var productTypes = CreateProductTypes(6);
      var products = CreateProducts(productTypes.Take(_sut.Capacity), _sut.Model);
      var p = MockProduct(_sut.Model, productTypes.Last());
      _mockRepo.ReplayAll();

      _sut.Initialize();
      ReceiveProducts(products);
      AssertScheduledItemsWithProducts(products);
      Assert.False(_sut.IsAvailable(p));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void MinimalValidExperimentConfiguration()
    {
      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();

      _sut = CreateMinimalConfiguredSUT();
      _sut.CurrentExperiment = experimentMock;
      _sut.Model = modelMock;

      _sut.Initialize();
    }

    [Test]
    public void Prefer_Second_ProductType_For_Sending()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var connection = _mockRepo.DynamicMock<IConnection>();
      var successor = _mockRepo.DynamicMock<IStationaryElement>();
      var productType1 = _mockRepo.DynamicMock<IProductType>();
      var productType2 = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType1);
      var product2 = MockProduct(modelMock, productType2);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connection.Expect(c => c.Destination)
                .Return(successor);
      connection.Expect(c => c.ProductTypes)
                .Return(new IProductType[0]);
      successor.Expect(se => se.IsAvailable(product1))
               .Return(false)
               .Repeat.Twice();
      successor.Expect(se => se.IsAvailable(product2))
               .Return(false)
               .Repeat.Once();
      successor.Expect(se => se.IsAvailable(product2))
               .Return(true)
               .Repeat.Twice();

      successor.GotAvailable += null;
      LastCall.IgnoreArguments();
      var eventRaiser = LastCall.GetEventRaiser();

      _mockRepo.ReplayAll();

      _entities.Add(successor);

      _sut = new Storage
              {
                Model = modelMock,
                CurrentExperiment = experimentMock
              };
      _sut.Add(connection);

      _sut.Initialize();
      _sut.Receive(product1);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1));
      scheduler.ProcessNextSchedulable();
      _sut.Receive(product2);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product2));
      scheduler.ProcessNextSchedulable();
      eventRaiser.Raise(connection, EventArgs.Empty);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product2));
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, _sut.Count);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Receive_And_Send_Immediatelly_If_Can_Send()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var connection = _mockRepo.DynamicMock<IConnection>();
      var successor = _mockRepo.DynamicMock<IStationaryElement>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product = MockProduct(modelMock, productType);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connection.Expect(c => c.Destination)
                .Return(successor);
      connection.Expect(c => c.ProductTypes)
                .Return(new IProductType[0]);
      successor.Expect(se => se.IsAvailable(product))
               .Return(true)
               .Repeat.Twice();

      _mockRepo.ReplayAll();

      _entities.Add(successor);

      _sut = new Storage
              {
                Model = modelMock,
                CurrentExperiment = experimentMock
              };
      _sut.Add(connection);

      _sut.Initialize();
      _sut.Receive(product);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product));
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(0, _sut.Count);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Remove_ProductTypeCapacity()
    {
      _sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeAmount>();
      _sut.AddProductTypeCapacity(item);
      _sut.RemoveProductTypeCapacity(item);
      Assert.IsEmpty(_sut.ProductTypeCapacities.ToArray());
    }

    [Test]
    public void Try_Send_Every_Kind_Of_ProductType_After_Connections_Got_Available()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var connection = _mockRepo.DynamicMock<IConnection>();
      var successor = _mockRepo.DynamicMock<IStationaryElement>();
      var productType1 = _mockRepo.DynamicMock<IProductType>();
      var productType2 = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType1);
      var product2 = MockProduct(modelMock, productType2);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connection.Expect(c => c.Destination)
                .Return(successor);
      connection.Expect(c => c.ProductTypes)
                .Return(new IProductType[0]);
      successor.Expect(se => se.IsAvailable(product1))
               .Return(false)
               .Repeat.Twice();
      successor.Expect(se => se.IsAvailable(product2))
               .Return(false)
               .Repeat.Once();
      successor.Expect(se => se.IsAvailable(product1))
               .Return(true)
               .Repeat.Twice();
      successor.Expect(se => se.IsAvailable(product2))
               .Return(true)
               .Repeat.Twice();

      successor.GotAvailable += null;
      LastCall.IgnoreArguments();
      var eventRaiser = LastCall.GetEventRaiser();

      _mockRepo.ReplayAll();

      _entities.Add(successor);

      _sut = CreateMinimalConfiguredSUT();
      _sut.HasLimitedCapacity = true;
      _sut.Model = modelMock;
      _sut.CurrentExperiment = experimentMock;
      _sut.Capacity = 2;
      _sut.Add(connection);

      _sut.Initialize();
      _sut.Receive(product1);
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1));
      scheduler.ProcessNextSchedulable();
      _sut.Receive(product2);
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product2));
      scheduler.ProcessNextSchedulable();
      eventRaiser.Raise(connection, EventArgs.Empty);
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product2));
      _mockRepo.VerifyAll();
    }
  }
}