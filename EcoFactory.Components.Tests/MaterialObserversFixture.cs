#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion


using Moq;
using NUnit.Framework;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public class MaterialObserversFixture
  {
    [Test]
    public void What_This_Test_Demonstrates()
    {
      var pwsMock = new Mock<IParallelWorkstation>();
      var sut = new MaterialObserverExtensions.ParallelWorkstation.Blocked();


      sut.Amount = 1d;
      Assert.AreEqual(1d, sut.Amount);
    }
  }
}