#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Resources;
using Milan.Simulation.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public abstract class WorkstationBaseFixture<T> : StationaryElementFixture<T>
    where T : WorkstationBase
  {
    protected override void Default_Ctor(T sut)
    {
      Assert.IsNull(sut.FailureDuration);
      Assert.IsNull(sut.BatchSize);
      Assert.IsNull(sut.FailureOccurrence);
      Assert.IsNull(sut.ProcessingDuration);
      Assert.IsNull(sut.SetupDuration);
      Assert.IsEmpty(sut.ProcessingDurations.ToArray());
      Assert.IsEmpty(sut.SetupDurations.ToArray());
      Assert.IsEmpty(sut.BatchSizes.ToArray());
      Assert.IsFalse(sut.HasSetup);
      Assert.IsFalse(sut.CanFail);
      base.Default_Ctor(sut);
    }

    [Test]
    public void Add_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddBatchSize(item);
      Assert.Contains(item, sut.BatchSizes.ToArray());
      Assert.AreEqual(1, sut.BatchSizes.Count());
    }
    
    [Test]
    public void Add_ProcessingDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddProcessing(item);
      Assert.Contains(item, sut.ProcessingDurations.ToArray());
      Assert.AreEqual(1, sut.ProcessingDurations.Count());
    }

    [Test]
    public void Add_Processing_Resource()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();

      sut.AddProcessingResource(resourceTypeAmount);

      Assert.IsTrue(sut.ProcessingResources.Count() == 1);
      Assert.IsTrue(sut.ProcessingResources.Contains(resourceTypeAmount));
    }

    [Test]
    public void Add_Processing_Resource_Twice_Fails()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();

      sut.AddProcessingResource(resourceTypeAmount);
      Assert.Throws<InvalidOperationException>(() => sut.AddProcessingResource(resourceTypeAmount));
    }

    [Test]
    public void Add_SetupDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddSetup(item);
      Assert.Contains(item, sut.SetupDurations.ToArray());
      Assert.AreEqual(1, sut.SetupDurations.Count());
    }

    [Test]
    public void Add_Setup_Resource()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();

      sut.AddSetupResource(resourceTypeAmount);

      Assert.IsTrue(sut.SetupResources.Count() == 1);
      Assert.IsTrue(sut.SetupResources.Contains(resourceTypeAmount));
    }

    [Test]    
    public void Add_Setup_Resource_Twice_Fails()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();

      sut.AddSetupResource(resourceTypeAmount);
      Assert.Throws<InvalidOperationException>(() => sut.AddSetupResource(resourceTypeAmount));
    }

    [Test]    
    public void Fail_On_Add_BatchSize_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddBatchSize(item);
      Assert.Throws<InvalidOperationException>(() => sut.AddBatchSize(item));
    }

    [Test]    
    public void Fail_On_Add_ProcessingDuration_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddProcessing(item);
      Assert.Throws<InvalidOperationException>(() => sut.AddProcessing(item));
    }

    [Test]    
    public void Fail_On_Add_Second_BatchSize_With_Same_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var item2 = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      item.Expect(i => i.ProductType)
          .Return(productTypeMock);
      item2.Expect(i => i.ProductType)
           .Return(productTypeMock);
      sut.AddBatchSize(item);
      sut.AddBatchSize(item2);
      Assert.Throws<InvalidOperationException>(() => _mockRepo.VerifyAll());
    }

    [Test]    
    public void Fail_On_Add_Second_ProcessingDuration_With_Same_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var item2 = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      item.Expect(i => i.ProductType)
          .Return(productTypeMock);
      item2.Expect(i => i.ProductType)
           .Return(productTypeMock);
      sut.AddProcessing(item);
      sut.AddProcessing(item2);
      Assert.Throws<InvalidOperationException>(() => _mockRepo.VerifyAll());
    }

    [Test]    
    public void Fail_On_Add_Second_SetupDuration_With_Same_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var item2 = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      item.Expect(i => i.ProductType)
          .Return(productTypeMock);
      item2.Expect(i => i.ProductType)
           .Return(productTypeMock);
      sut.AddSetup(item);
      sut.AddSetup(item2);
      Assert.Throws<InvalidOperationException>(() => _mockRepo.VerifyAll());
    }

    [Test]    
    public void Fail_On_Add_SetupDuration_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddSetup(item);
      Assert.Throws<InvalidOperationException>(() => sut.AddSetup(item));
    }

    [Test]    
    public void Fail_On_Remove_Non_Existing_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveBatchSize(item));
    }

    [Test]    
    public void Fail_On_Remove_Non_Existing_ProcessingDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveProcessing(item));
    }

    [Test]    
    public void Fail_On_Remove_Non_Existing_Processing_Resource()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveProcessingResource(item));
    }

    [Test]    
    public void Fail_On_Remove_Non_Existing_SetupDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveSetup(item));
    }

    [Test]    
    public void Fail_On_Remove_Non_Existing_Setup_Resource()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveSetupResource(item));
    }

    [Test]    
    public void Fail_On_Remove_Null_Fro_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      IProductTypeDistribution item = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveBatchSize(item));
    }

    [Test]    
    public void Fail_On_Remove_Null_Fro_ProcessingDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      IProductTypeDistribution item = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveProcessing(item));
    }

    [Test]    
    public void Fail_On_Remove_Null_Fro_Processing_Resources()
    {
      var sut = CreateMinimalConfiguredSUT();
      IResourcePoolResourceTypeAmount item = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveProcessingResource(item));
    }

    [Test]    
    public void Fail_On_Remove_Null_Fro_SetupDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      IProductTypeDistribution item = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveSetup(item));
    }

    [Test]    
    public void Fail_On_Remove_Null_Fro_Setup_Resources()
    {
      var sut = CreateMinimalConfiguredSUT();
      IResourcePoolResourceTypeAmount item = null;
      Assert.Throws<InvalidOperationException>(() => sut.RemoveSetupResource(item));
    }

    [Test]
    public void Remove_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddBatchSize(item);
      sut.RemoveBatchSize(item);
      Assert.IsEmpty(sut.BatchSizes.ToArray());
    }

    [Test]
    public void Remove_ProcessingDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddProcessing(item);
      sut.RemoveProcessing(item);
      Assert.IsEmpty(sut.ProcessingDurations.ToArray());
    }

    [Test]
    public void Remove_Processing_Resource()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();

      sut.AddProcessingResource(resourceTypeAmount);

      Assert.IsTrue(sut.ProcessingResources.Count() == 1);
      Assert.IsTrue(sut.ProcessingResources.Contains(resourceTypeAmount));

      sut.RemoveProcessingResource(resourceTypeAmount);

      Assert.IsFalse(sut.ProcessingResources.Any());
      Assert.IsFalse(sut.ProcessingResources.Contains(resourceTypeAmount));
    }

    [Test]
    public void Remove_SetupDuration()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddSetup(item);
      sut.RemoveSetup(item);
      Assert.IsEmpty(sut.SetupDurations.ToArray());
    }

    [Test]
    public void Remove_Setup_Resource()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = _mockRepo.DynamicMock<IResourcePoolResourceTypeAmount>();

      sut.AddSetupResource(resourceTypeAmount);

      Assert.IsTrue(sut.SetupResources.Count() == 1);
      Assert.IsTrue(sut.SetupResources.Contains(resourceTypeAmount));

      sut.RemoveSetupResource(resourceTypeAmount);

      Assert.IsFalse(sut.SetupResources.Any());
      Assert.IsFalse(sut.SetupResources.Contains(resourceTypeAmount));
    }

    [Test]
    public void Set_BatchSize()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetProperty("BatchSize", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.BatchSize = v, s => s.BatchSize);
    }

    [Test]
    public void Set_CanFail()
    {
      var value = true;
      SetProperty("CanFail", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.CanFail = v, s => s.CanFail);
    }

    [Test]
    public void Set_FailureDuration()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetProperty("FailureDuration", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.FailureDuration = v, s => s.FailureDuration);
    }

    [Test]
    public void Set_FailureOccurrence()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetProperty("FailureOccurrence", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.FailureOccurrence = v, s => s.FailureOccurrence);
    }

    [Test]
    public void Set_HasSetup()
    {
      var value = true;
      SetProperty("HasSetup", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.HasSetup = v, s => s.HasSetup);
    }

    [Test]
    public void Set_ProcessingDuration()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetProperty("ProcessingDuration", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.ProcessingDuration = v, s => s.ProcessingDuration);
    }

    [Test]
    public void Set_Same_BatchSize_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.BatchSize = v);
    }

    [Test]
    public void Set_Same_CanFail_Does_Not_Raise_PropertyChanged()
    {
      var value = true;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.CanFail = v);
    }

    [Test]
    public void Set_Same_FailureDuration_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.FailureDuration = v);
    }

    [Test]
    public void Set_Same_FailureOccurrence_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.FailureOccurrence = v);
    }

    [Test]
    public void Set_Same_HasSetup_Does_Not_Raise_PropertyChanged()
    {
      var value = true;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.HasSetup = v);
    }

    [Test]
    public void Set_Same_ProcessingDuration_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.ProcessingDuration = v);
    }

    [Test]
    public void Set_Same_SetupDuration_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.SetupDuration = v);
    }

    [Test]
    public void Set_SetupDuration()
    {
      var value = _mockRepo.Stub<DistributionConfiguration>();
      SetProperty("SetupDuration", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.SetupDuration = v, s => s.SetupDuration);
    }
  }
}