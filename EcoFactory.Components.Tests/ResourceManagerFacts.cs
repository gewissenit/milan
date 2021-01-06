using EcoFactory.Components.States;
using GeWISSEN.Utils;
using Milan.Simulation.Resources;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  internal class ResourceManagerFacts
  {
    private ResourceManager SUT { get; set; }

    //TODO rename dependencies when their purpose is clear
    private Mock<IState> GivenInfiniteState { get; set; }
    private Action GivenAction { get; set; }

    private Mock<IResourcePool>[] GivenResourcePools { get; set; }
    private Mock<IResourceType>[] GivenResourceTypes { get; set; }
    private List<Resource> _givenResources;
    private Dictionary<IResourcePool, IDictionary<IResourceType, int>> GivenRequestedResources { get; set; }

    bool _wasCalled = false;

    [SetUp]
    public void Setup()
    {
      _givenResources = new List<Resource>();
    }

    [TearDown]
    public void TearDown()
    {
      GivenResourcePools = null;
      GivenResourceTypes = null;
      GivenRequestedResources = null;
      _givenResources = null;
      GivenInfiniteState = null;
      GivenAction = null;
    }

    [Test]
    public void It_depends_on_an_infinite_state()
    {
      Assert.Throws<ArgumentNullException>(()=>new ResourceManager(null, Do.Nothing));
    }

    [Test]
    public void It_depends_on_an_action()
    {
      Assert.Throws<ArgumentNullException>(() => new ResourceManager(new Mock<IState>().Object, null));
    }

    [Test]
    public void Its_required_resources_can_be_defined()
    {
      GivenDefaultDependencies();
      WhenSutIsCreated();

      GivenItRequiresASingleResourceFromASinglePool(); //<---!

      ThenItsRequestedResourcesAreTheGivenRequestedResource();
    }

    [Test]
    public void It_asks_the_pool_for_the_required_resource_type_when_it_needs_it()
    {
      GivenDefaultDependencies();
      WhenSutIsCreated();
      GivenItRequiresASingleResourceFromASinglePool();

      WhenItNeedsItsRequiredResources(); //<---!

      ThenItAsksThePoolIfItHasTheRequiredResourceTypeAvailable();
    }

    [Test]
    public void It_does_nothing_when_it_has_no_required_resources()
    {
      GivenDefaultDependencies();
      WhenSutIsCreated();
      GivenItRequiresNothing();

      WhenItNeedsItsRequiredResources(); //<---!

      ThenItNeverAsksThePoolForAnything();
    }
    
    [Test]
    public void It_retrieves_the_specified_resource_from_its_pool_when_it_needs_it_and_it_is_available()
    {
      GivenDefaultDependencies();
      WhenSutIsCreated();
      GivenItRequiresASingleResourceFromASinglePool();
      WhenThePoolHasTheRequiredResourceAvailable();

      WhenItNeedsItsRequiredResources(); //<---!

      ThenItRetrievesItsRequiredResourceFromThePool();
      ThenItsAvailableResourcesContainOnlyTheRequestedResource();
    }
    
    [Test]
    public void It_calls_the_given_action_when_instructed_to_enter_processing_and_all_required_resources_are_available()
    {
      It_retrieves_the_specified_resource_from_its_pool_when_it_needs_it_and_it_is_available();

      WhenItIsInstructedToEnterTheProductionProcess();

      ThenTheGivenActionWasCalled();
    }


    [Test]
    public void It_starts_waiting_for_returning_resources_when_the_pool_has_not_enough_available_resources()
    {
      GivenDefaultDependencies();
      WhenSutIsCreated();
      GivenItRequiresASingleResourceFromASinglePool();
      WhenThePoolDoesNotHaveTheRequiredResourceAvailable();

      WhenItNeedsItsRequiredResources(); //<---!

      Assert.IsFalse(SUT.ResourcesAvailable);
      GivenInfiniteState.Verify(m => m.Enter(), Times.Once);

      WhenThePoolHasTheRequiredResourceAvailable();
      WhenThePoolSignalsReceptionOfAResource();

      Assert.IsTrue(SUT.ResourcesAvailable);
      GivenInfiniteState.Verify(m => m.Exit(), Times.Once);
    }

    private void WhenThePoolSignalsReceptionOfAResource()
    {
      GivenResourcePools[0].Raise(m => m.ResourcesReceived += null, null, null);
    }

    private void WhenSutIsCreated()
    {
      SUT = new ResourceManager(GivenInfiniteState.Object, GivenAction);
    }

    private void GivenDefaultDependencies()
    {
      GivenInfiniteState = new Mock<IState>();
      GivenAction = () => _wasCalled = true;

      GivenResourcePools = new[] { new Mock<IResourcePool>() };
      GivenResourceTypes = new[] { new Mock<IResourceType>() };
    }
    
    private void GivenItRequiresASingleResourceFromASinglePool()
    {
      GivenRequestedResources = new Dictionary<IResourcePool, IDictionary<IResourceType, int>>()
      {
        { GivenResourcePools[0].Object, new Dictionary<IResourceType, int>()
                                        {
                                          { GivenResourceTypes[0].Object, 1 }
                                        }
        }
      };

      SUT.RequestedResources = GivenRequestedResources;
    }

    private void GivenItRequiresNothing()
    {
      SUT.RequestedResources = new Dictionary<IResourcePool, IDictionary<IResourceType, int>>();
    }

    private void WhenItNeedsItsRequiredResources()
    {
      SUT.HandleResources();
    }

    private void WhenItIsInstructedToEnterTheProductionProcess()
    {
      SUT.EnterProductionProcess();
    }

    private void WhenThePoolHasTheRequiredResourceAvailable()
    {
      GivenResourcePools[0].Setup(m => m.HasAvailable(GivenResourceTypes[0].Object, 1)).Returns(true);
      GivenResourcePools[0].Setup(m => m.GetResources(GivenResourceTypes[0].Object, It.IsAny<int>()))
                           .Returns((IResourceType rt, int count)=>GenerateResourcesOfRequestedType(rt, count));
    }

    private void WhenThePoolDoesNotHaveTheRequiredResourceAvailable()
    {
      GivenResourcePools[0].Setup(m => m.HasAvailable(GivenResourceTypes[0].Object, 1)).Returns(false);      
    }

    private void ThenItsRequestedResourcesAreTheGivenRequestedResource()
    {
      Assert.AreSame(GivenRequestedResources, SUT.RequestedResources);
    }

    private void ThenItAsksThePoolIfItHasTheRequiredResourceTypeAvailable()
    {
      GivenResourcePools[0].Verify(m => m.HasAvailable(GivenResourceTypes[0].Object, 1));
    }

    private void ThenItRetrievesItsRequiredResourceFromThePool()
    {
      GivenResourcePools[0].Verify(m => m.GetResources(GivenResourceTypes[0].Object, 1));
    }

    private void ThenItNeverAsksThePoolForAnything()
    {
      GivenResourcePools[0].Verify(m => m.HasAvailable(It.IsAny<IResourceType>(), It.IsAny<int>()), Times.Never);
    }

    private void ThenItsAvailableResourcesContainOnlyTheRequestedResource()
    {
      Assert.AreSame(_givenResources[0], SUT.AvailableResources.Single());
    }

    private void ThenTheGivenActionWasCalled()
    {
      Assert.IsTrue(_wasCalled);
    }

    private IEnumerable<Resource> GenerateResourcesOfRequestedType(IResourceType rt, int count)
    {
      // generate count Resource instance, linked to the default pool
      return Enumerable.Range(1, count).Select(_ =>
      {
        var r = new Resource(rt, GivenResourcePools[0].Object, () => 0);
        _givenResources.Add(r);
        return r;
      });
    }
  }
}
