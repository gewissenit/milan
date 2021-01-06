#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using Emporer.Math.Distribution;
using Milan.Simulation.Resources.Events;
using Milan.Simulation.Tests;
using Moq;
using NUnit.Framework;

namespace Milan.Simulation.Resources.Tests
{
  [TestFixture]
  public class ResourcePoolFixture
  {
    private ResourcePool CreateSUT()
    {
      return new ResourcePool();
    }

    [Test]
    public void Add_Resource_Type_Amount()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();

      sut.Add(resourceTypeAmount.Object);

      Assert.IsTrue(sut.Resources.Count() == 1);
      Assert.IsTrue(sut.Resources.Contains(resourceTypeAmount.Object));
    }


    [Test]
    public void Has_Available()
    {
      var sut = CreateSUT();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmountRequest = new Mock<IResourceTypeAmount>();

      var resourceTypeAmountSut = CreateResourceTypeAmount(resourceTypeMock, 5, sut);

      resourceTypeAmountRequest.Setup(rta => rta.Amount)
                               .Returns(2);
      resourceTypeAmountRequest.Setup(rta => rta.ResourceType)
                               .Returns(resourceTypeMock.Object);

      sut.Initialize();

      Assert.IsTrue(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      resourceTypeMock.Verify();
      resourceTypeAmountRequest.Verify();
      resourceTypeAmountSut.Verify();
    }

    [Test]
    public void Resource_Maintenance_Start_And_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = new Mock<IExperiment>();
      experimentMock.Setup(e => e.Scheduler)
                    .Returns(scheduler);
      var sut = CreateSUT();

      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmountRequest = new Mock<IResourceTypeAmount>();

      var resourceTypeAmountSut = CreateResourceTypeAmount(resourceTypeMock, 1, sut);
      var resource = sut.AvailableResources.First();
      var maintenanceDuration = new Mock<IRealDistribution>();
      maintenanceDuration.Setup(d => d.GetSample())
                         .Returns(1);
      var usageAmount = new Mock<IRealDistribution>();
      usageAmount.Setup(d => d.GetSample())
                 .Returns(1);
      resource.MaintenanceDuration = maintenanceDuration.Object;
      resource.UsageAmount = usageAmount.Object;

      resourceTypeAmountRequest.Setup(rta => rta.Amount)
                               .Returns(1);
      resourceTypeAmountRequest.Setup(rta => rta.ResourceType)
                               .Returns(resourceTypeMock.Object);
      sut.CurrentExperiment = experimentMock.Object;

      sut.Initialize();

      Assert.IsTrue(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      sut.GetResources(resourceTypeAmountRequest.Object);

      Assert.IsFalse(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      sut.Return(resource);

      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1,
                      scheduler.OfType<ResourceMaintenanceStartEvent>()
                               .Count());
      Assert.AreEqual(1,
                      scheduler.OfType<ResourceMaintenanceEndEvent>()
                               .Count());
      Assert.IsFalse(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.IsTrue(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      resourceTypeMock.Verify();
      experimentMock.Verify();
      maintenanceDuration.Verify();
      usageAmount.Verify();
      resourceTypeAmountRequest.Verify();
      resourceTypeAmountSut.Verify();
    }

    private static Mock<IResourceTypeAmount> CreateResourceTypeAmount(Mock<IResourceType> resourceTypeMock, int amount, ResourcePool sut)
    {
      var resourceTypeAmountSut = new Mock<IResourceTypeAmount>();
      resourceTypeAmountSut.Setup(rta => rta.Amount)
                           .Returns(amount);
      resourceTypeAmountSut.Setup(rta => rta.ResourceType)
                           .Returns(resourceTypeMock.Object);
      for (int i = 0; i < amount; i++)
      {
        //todo: mock resource
        sut.AvailableResources.Add(new Resource(resourceTypeMock.Object, sut, () => 0));
      }
      sut.Add(resourceTypeAmountSut.Object);
      return resourceTypeAmountSut;
    }

    [Test]
    public void Remove_Resource_Type_Amount()
    {
      var sut = CreateSUT();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();

      sut.Add(resourceTypeAmount.Object);

      Assert.IsTrue(sut.Resources.Count() == 1);
      Assert.IsTrue(sut.Resources.Contains(resourceTypeAmount.Object));

      sut.Remove(resourceTypeAmount.Object);

      Assert.IsFalse(sut.Resources.Any());
      Assert.IsFalse(sut.Resources.Contains(resourceTypeAmount.Object));
    }

    [Test]
    public void Resources_Are_Assigned()
    {
      var sut = CreateSUT();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmountRequest = new Mock<IResourceTypeAmount>();
      var experimentMock = new Mock<IExperiment>();
      experimentMock.Setup(em => em.CurrentTime)
                    .Returns(1000);
      sut.CurrentExperiment = experimentMock.Object;
      var resourceTypeAmountSut = CreateResourceTypeAmount(resourceTypeMock, 3, sut);
      resourceTypeAmountRequest.Setup(rta => rta.Amount)
                               .Returns(2);
      resourceTypeAmountRequest.Setup(rta => rta.ResourceType)
                               .Returns(resourceTypeMock.Object);
      sut.Initialize();

      Assert.IsTrue(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      var resources = sut.GetResources(resourceTypeAmountRequest.Object)
                         .ToArray();

      Assert.IsTrue(resources.Count() == 2);
      Assert.IsFalse(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));
      Assert.IsTrue(resources.All(r => r.ResourceType == resourceTypeMock.Object));

      resourceTypeMock.Verify();
      resourceTypeAmountRequest.Verify();
      resourceTypeAmountSut.Verify();
    }

    [Test]
    public void Resources_Are_Returned()
    {
      var sut = CreateSUT();
      var experimentMock = new Mock<IExperiment>();
      experimentMock.Setup(em => em.CurrentTime)
                    .Returns(1000);
      sut.CurrentExperiment = experimentMock.Object;
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmountRequest = new Mock<IResourceTypeAmount>();

      var resourceTypeAmountSut = CreateResourceTypeAmount(resourceTypeMock, 3, sut);

      resourceTypeAmountRequest.Setup(rta => rta.Amount)
                               .Returns(2);
      resourceTypeAmountRequest.Setup(rta => rta.ResourceType)
                               .Returns(resourceTypeMock.Object);

      sut.Initialize();

      Assert.IsTrue(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      var resources = sut.GetResources(resourceTypeAmountRequest.Object)
                         .ToList();

      Assert.IsTrue(resources.Count() == 2);
      Assert.IsTrue(resources.All(r => r.ResourceType == resourceTypeMock.Object));

      Assert.IsFalse(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      resources.ForEach(sut.Return);

      Assert.IsTrue(sut.HasAvailable(resourceTypeAmountRequest.Object.ResourceType, resourceTypeAmountRequest.Object.Amount));

      resourceTypeMock.Verify();
      resourceTypeAmountRequest.Verify();
      resourceTypeAmountSut.Verify();
    }

    [Test]
    public void Set_Name()
    {
      var sut = CreateSUT();
      var check = "sut";
      sut.Name = check;
      Assert.AreEqual(check, sut.Name);
    }
  }
}