#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class ProductTransmitterFixture
  {
    private Product MockProduct(IModel model, IProductType productType)
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

    [SetUp]
    public void Setup()
    {
      _mockRepo = new MockRepository();
    }


    [TearDown]
    public void TearDown()
    {
      _mockRepo = null;
    }

    protected MockRepository _mockRepo;

    private IConnection GetConnectionWithPriority(int prio, IStationaryElement destination, IProductType productType)
    {
      var mockConnection = _mockRepo.StrictMock<IConnection>();
      mockConnection.Expect(mC => mC.Destination)
                    .Return(destination)
                    .Repeat.Any();
      mockConnection.Expect(mC => mC.ProductTypes)
                    .Return(new[]
                            {
                              productType
                            })
                    .Repeat.Once();
      mockConnection.Expect(mC => mC.Priority)
                    .Return(prio)
                    .Repeat.Once();
      return mockConnection;
    }

    [Test]
    public void Can_Transmit_If_Single_Receiver_Is_Available()
    {
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var mockStationaryElement = _mockRepo.StrictMock<IStationaryElement>();
      var mockConnection = _mockRepo.StrictMock<IConnection>();
      var mockConnections = new[]
                            {
                              mockConnection
                            };
      var mockProduct = MockProduct(modelMock, productType);
      
      mockConnection.Expect(mC => mC.Destination)
                    .Return(mockStationaryElement)
                    .Repeat.Twice();
      mockConnection.Expect(mC => mC.ProductTypes)
                    .Return(new[]
                            {
                              productType
                            })
                    .Repeat.Once();
      mockStationaryElement.Expect(mSE => mSE.IsAvailable(mockProduct))
                           .Return(true)
                           .Repeat.Once();

      _mockRepo.ReplayAll();

      var sut = new ProductTransmitter(mockConnections);
      Assert.IsTrue(sut.CanTransmit(mockProduct));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void List_Of_Receivers_Contains_The_Right_Set_Of_Elements_After_Ctor()
    {
      var mockDestination1 = _mockRepo.DynamicMock<IStationaryElement>();
      var mockDestination2 = _mockRepo.DynamicMock<IStationaryElement>();

      var mockConnection1 = _mockRepo.DynamicMock<IConnection>();
      var mockConnection2 = _mockRepo.DynamicMock<IConnection>();

      var mockConnections = new[]
                            {
                              mockConnection1, mockConnection2
                            };

      mockConnection1.Expect(mC => mC.Destination)
                     .Return(mockDestination1)
                     .Repeat.Once();
      mockConnection2.Expect(mC => mC.Destination)
                     .Return(mockDestination2)
                     .Repeat.Once();

      _mockRepo.ReplayAll();

      var sut = new ProductTransmitter(mockConnections);

      Assert.IsTrue(sut.Receivers.Contains(mockDestination1));
      Assert.IsTrue(sut.Receivers.Contains(mockDestination2));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void List_Of_Receivers_Is_Not_Set_By_Reference_During_Ctor()
    {
      var mockStationaryElement = _mockRepo.DynamicMock<IStationaryElement>();
      var mockConnection = _mockRepo.DynamicMock<IConnection>();
      var mockConnections = new[]
                            {
                              mockConnection
                            };
      var sut = new ProductTransmitter(mockConnections);

      mockConnection.Expect(mC => mC.Destination)
                    .Return(mockStationaryElement)
                    .Repeat.Once();

      Assert.AreNotSame(mockConnections, sut.Receivers);
    }

    [Test]
    public void Transmits_If_Single_Receiver_Is_Available()
    {
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var mockStationaryElement = _mockRepo.StrictMock<IStationaryElement>();
      var mockConnection = _mockRepo.StrictMock<IConnection>();
      var mockConnections = new[]
                            {
                              mockConnection
                            };
      var productType = _mockRepo.DynamicMock<IProductType>();
      var mockProduct = MockProduct(modelMock, productType);
     
      mockConnection.Expect(mC => mC.Destination)
                    .Return(mockStationaryElement)
                    .Repeat.Times(3);
      mockConnection.Expect(mC => mC.ProductTypes)
                    .Return(new[]
                            {
                              productType
                            })
                    .Repeat.Once();
      mockConnection.Expect(mC => mC.Priority)
                    .Return(int.MinValue)
                    .Repeat.Once();
      mockStationaryElement.Expect(mSE => mSE.IsAvailable(mockProduct))
                           .Return(true)
                           .Repeat.Once();
      mockStationaryElement.Expect(mSE => mSE.Receive(mockProduct))
                           .Repeat.Once();

      _mockRepo.ReplayAll();

      var sut = new ProductTransmitter(mockConnections);
      sut.Transmit(mockProduct);

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Transmits_To_Destination_With_Highest_Priority()
    {
      var productType = _mockRepo.DynamicMock<IProductType>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      
      var mockSeHigh = _mockRepo.StrictMock<IStationaryElement>();
      var mockSeNormal = _mockRepo.StrictMock<IStationaryElement>();
      var mockSeLow = _mockRepo.StrictMock<IStationaryElement>();

      var mockConnectionHighPrio = GetConnectionWithPriority(3, mockSeHigh, productType);
      var mockConnectionNormalPrio = GetConnectionWithPriority(2, mockSeNormal, productType);
      var mockConnectionLowPrio = GetConnectionWithPriority(1, mockSeLow, productType);

      var mockConnections = new[]
                            {
                              mockConnectionNormalPrio, mockConnectionLowPrio, mockConnectionHighPrio
                            };

      var mockProduct = MockProduct(modelMock, productType);
      
      mockSeHigh.Expect(mSE => mSE.IsAvailable(mockProduct))
                .Return(true)
                .Repeat.Once();
      mockSeNormal.Expect(mSE => mSE.IsAvailable(mockProduct))
                  .Return(true)
                  .Repeat.Once();
      mockSeLow.Expect(mSE => mSE.IsAvailable(mockProduct))
               .Return(true)
               .Repeat.Once();

      mockSeLow.Expect(mSE => mSE.Receive(mockProduct))
               .Repeat.Once();

      _mockRepo.ReplayAll();

      var sut = new ProductTransmitter(mockConnections);
      sut.Transmit(mockProduct);

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Transmits_To_Same_Destination_As_Long_As_It_Is_Available()
    {
      //TODO: this has to be implemented
      var productType = _mockRepo.DynamicMock<IProductType>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var mockSeHigh = _mockRepo.StrictMock<IStationaryElement>();
      var mockSeNormal = _mockRepo.StrictMock<IStationaryElement>();
      var mockSeLow = _mockRepo.StrictMock<IStationaryElement>();

      var mockConnectionHighPrio = GetConnectionWithPriority(3, mockSeHigh, productType);
      var mockConnectionNormalPrio = GetConnectionWithPriority(2, mockSeNormal, productType);
      var mockConnectionLowPrio = GetConnectionWithPriority(1, mockSeLow, productType);

      var mockConnections = new[]
                            {
                              mockConnectionNormalPrio, mockConnectionLowPrio, mockConnectionHighPrio
                            };

      var mockProduct = MockProduct(modelMock, productType);
      
      mockSeHigh.Expect(mSE => mSE.IsAvailable(mockProduct))
                .Return(true)
                .Repeat.Once();
      mockSeNormal.Expect(mSE => mSE.IsAvailable(mockProduct))
                  .Return(true)
                  .Repeat.Once();
      mockSeLow.Expect(mSE => mSE.IsAvailable(mockProduct))
               .Return(true)
               .Repeat.Once();

      mockSeLow.Expect(mSE => mSE.Receive(mockProduct))
               .Repeat.Once();

      _mockRepo.ReplayAll();

      var sut = new ProductTransmitter(mockConnections);
      sut.Transmit(mockProduct);

      _mockRepo.VerifyAll();
    }
  }
}