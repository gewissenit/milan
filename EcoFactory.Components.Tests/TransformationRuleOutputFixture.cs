#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.JsonStore.Tests;
using Milan.Simulation;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public class TransformationRuleOutputFixture : DomainEntityFixture<TransformationRuleOutput>
  {
    [SetUp]
    public void SetUp()
    {
      _MockRepo = new MockRepository();
    }

    [TearDown]
    public void TearDown()
    {
    }

    private MockRepository _MockRepo;

    protected override TransformationRuleOutput CreateSUT()
    {
      return new TransformationRuleOutput();
    }

    [Test]
    public void Add_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmountMock = _MockRepo.DynamicMock<IProductTypeAmount>();
      sut.Add(productTypeAmountMock);
      Assert.Contains(productTypeAmountMock, sut.Outputs.ToArray());
      Assert.AreEqual(1, sut.Outputs.Count());
    }

    [Test]
    public void Default_Ctor()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.AreEqual(null, sut.Distribution);
      Assert.AreEqual(null, sut.ProcessingDuration);
      Assert.IsEmpty(sut.Outputs.ToArray());
    }

    [Test]
    public void Fail_On_Add_Output_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmountMock = _MockRepo.DynamicMock<IProductTypeAmount>();
      sut.Add(productTypeAmountMock);
      Assert.Throws<InvalidOperationException>(() => sut.Add(productTypeAmountMock));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmountMock = _MockRepo.DynamicMock<IProductTypeAmount>();
      Assert.Throws<InvalidOperationException>(() => sut.Remove(productTypeAmountMock));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      IProductTypeAmount productTypeAmountMock = null;
      Assert.Throws<InvalidOperationException>(() => sut.Remove(productTypeAmountMock));
    }

    [Test]
    public void Remove_Output()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productTypeAmountMock = _MockRepo.DynamicMock<IProductTypeAmount>();
      sut.Add(productTypeAmountMock);
      sut.Remove(productTypeAmountMock);
      Assert.IsEmpty(sut.Outputs.ToArray());
    }

    [Test]
    public void Set_Probability()
    {
      var value = 5;
      SetProperty("Probability", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Probability = v, s => s.Probability);
    }

    [Test]
    public void Set_ProcessingDuration()
    {
      var value = _MockRepo.DynamicMock<DistributionConfiguration>();
      SetProperty("ProcessingDuration", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.ProcessingDuration = v, s => s.ProcessingDuration);
    }

    [Test]
    public void Set_Same_Probability_Does_Not_Raise_PropertyChanged()
    {
      var value = 4;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Probability = v);
    }

    [Test]
    public void Set_Same_ProcessingDuration_Does_Not_Raise_PropertyChanged()
    {
      var value = _MockRepo.DynamicMock<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.ProcessingDuration = v);
    }
  }
}