#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class TransmitterFixture
  {
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

    /// <summary>
    ///   Mock type to be used as generic parameter of Transmitter.
    /// </summary>
    public class MockType
    {
    }

    [Test]
    public void Can_Not_Transmit_If_All_Receivers_Are_Not_Available()
    {
      var mockPacket = _mockRepo.Stub<MockType>();

      var mockReceiver1 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiver2 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiver3 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiverList = new[]
                             {
                               mockReceiver1, mockReceiver2, mockReceiver3
                             };

      mockReceiver1.Expect(r => r.IsAvailable(mockPacket))
                   .Return(false);
      mockReceiver2.Expect(r => r.IsAvailable(mockPacket))
                   .Return(false);
      mockReceiver3.Expect(r => r.IsAvailable(mockPacket))
                   .Return(false);

      _mockRepo.ReplayAll();
      var sut = new Transmitter<MockType>(mockReceiverList);

      Assert.IsFalse(sut.CanTransmit(mockPacket));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Can_Not_Transmit_If_Empty_Receivers_List()
    {
      var mockPacket = _mockRepo.Stub<MockType>();
      var sut = new Transmitter<MockType>(new IReceiver<MockType>[]
                                          {
                                          });
      Assert.IsFalse(sut.CanTransmit(mockPacket));
    }

    [Test]
    public void Can_Transmit_If_At_Least_One_Receiver_Is_Available()
    {
      var mockPacket = _mockRepo.Stub<MockType>();

      var mockReceiver1 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiver2 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiver3 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiverList = new[]
                             {
                               mockReceiver1, mockReceiver2, mockReceiver3
                             };

      mockReceiver1.Expect(r => r.IsAvailable(mockPacket))
                   .Return(false)
                   .Repeat.Once();
      mockReceiver2.Expect(r => r.IsAvailable(mockPacket))
                   .Return(true)
                   .Repeat.Once();
      // mockReceiver3.IsAvailable should not be called (2 was true, step out early!)

      _mockRepo.ReplayAll();
      var sut = new Transmitter<MockType>(mockReceiverList);

      Assert.IsTrue(sut.CanTransmit(mockPacket));
      _mockRepo.VerifyAll();
    }

    [Test]
    public void List_Of_Receivers_Contains_The_Right_Set_Of_Elements_After_Ctor()
    {
      var mockReceiver1 = _mockRepo.Stub<IReceiver<MockType>>();
      var mockReceiver2 = _mockRepo.Stub<IReceiver<MockType>>();

      var mockReceiverList = new[]
                             {
                               mockReceiver1, mockReceiver2
                             };

      var sut = new Transmitter<MockType>(mockReceiverList);

      Assert.Contains(mockReceiver2, sut.Receivers.ToArray());
      Assert.Contains(mockReceiver1, sut.Receivers.ToArray());
    }

    [Test]
    public void List_Of_Receivers_Is_Not_Set_By_Reference_During_Ctor()
    {
      var mockReceiverList = new IReceiver<MockType>[]
                             {
                             };
      var sut = new Transmitter<MockType>(mockReceiverList);

      Assert.AreNotSame(mockReceiverList, sut.Receivers);
    }

    [Test]
    public void On_Transmit_Available_Receiver_Receives_Message()
    {
      var mockPacket = _mockRepo.Stub<MockType>();
      var mockReceiver1 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiverList = new[]
                             {
                               mockReceiver1
                             };

      mockReceiver1.Expect(r => r.IsAvailable(mockPacket))
                   .Return(true)
                   .Repeat.Once();
      mockReceiver1.Expect(r => r.Receive(mockPacket))
                   .Repeat.Once();
      var sut = new Transmitter<MockType>(mockReceiverList);

      _mockRepo.ReplayAll();

      sut.Transmit(mockPacket);

      _mockRepo.VerifyAll();
    }

    [Test]
    public void On_Transmit_OnTransmit_Is_Called()
    {
      var mockPacket = _mockRepo.Stub<MockType>();
      var mockReceiver1 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiverList = new[]
                             {
                               mockReceiver1
                             };

      mockReceiver1.Expect(r => r.IsAvailable(mockPacket))
                   .Return(true)
                   .Repeat.Once();
      mockReceiver1.Expect(r => r.Receive(mockPacket))
                   .Repeat.Once();
      var sut = new Transmitter<MockType>(mockReceiverList);

      var wasCalled = false;
      sut.OnTransmit = () =>
                       {
                         wasCalled = true;
                       };


      _mockRepo.ReplayAll();

      sut.Transmit(mockPacket);

      _mockRepo.VerifyAll();
      Assert.IsTrue(wasCalled, "The delagate 'OnTransmit' was not called.");
    }

    [Test]
    public void On_Transmit_Only_One_Available_Receiver_Receives_Message()
    {
      var mockPacket = _mockRepo.Stub<MockType>();
      var mockReceiver1 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiver2 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiver3 = _mockRepo.DynamicMock<IReceiver<MockType>>();
      var mockReceiverList = new[]
                             {
                               mockReceiver1, mockReceiver2, mockReceiver3
                             };

      mockReceiver1.Expect(r => r.IsAvailable(mockPacket))
                   .Return(true)
                   .Repeat.Once();
      mockReceiver1.Expect(r => r.Receive(mockPacket))
                   .Repeat.Once();
      var sut = new Transmitter<MockType>(mockReceiverList);

      _mockRepo.ReplayAll();

      sut.Transmit(mockPacket);

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Throw_ArgumentNullException_For_Null_Message_In_CanTransmit()
    {
      var sut = new Transmitter<MockType>(new IReceiver<MockType>[]
                                          {
                                          });
      Assert.Throws<ArgumentNullException>(() => sut.CanTransmit(null));
    }

    [Test]
    public void Throw_ArgumentNullException_For_Null_Message_In_Transmit()
    {
      var sut = new Transmitter<MockType>(new IReceiver<MockType>[]
                                          {
                                          });
      Assert.Throws<ArgumentNullException>(() => sut.Transmit(null));
    }

    [Test]
    public void Throw_ArgumentNullException_For_Null_Receivers_In_Ctor()
    {
      Assert.Throws<ArgumentNullException>(() => new Transmitter<MockType>(null));
    }
  }
}