using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milan.Simulation.Resources.Tests
{
  [TestFixture]
  public class ResourceTypeInfluenceFacts
  {
    private IResourceType SUT { get; set; }
    private IResourceTypeInfluence _influence;

    [Test]
    public void It_provides_a_list_of_influences()
    {
      WhenSutIsCreated();
      ThenItHasAnEmptyListOfInfluences();
    }

    [Test]
    public void Influences_can_be_added()
    {
      GivenAnInfluence();

      WhenSutIsCreated();

      WhenTheGivenInfluenceIsAdded();

      ThenItContainsExactlyTheGivenInfluence();
    }

    [Test]
    public void Influences_can_be_removed()
    {
      Influences_can_be_added();
      WhenTheGivenInflueneIsRemoved();
      ThenItHasAnEmptyListOfInfluences();
    }

    private void GivenAnInfluence()
    {
      _influence = Mock.Of<IResourceTypeInfluence>();
    }

    private void WhenSutIsCreated()
    {
      SUT = new ResourceType();
    }

    private void WhenTheGivenInflueneIsRemoved()
    {
      SUT.Remove(_influence);
    }

    private void WhenTheGivenInfluenceIsAdded()
    {
      SUT.Add(_influence);
    }

    private void ThenItContainsExactlyTheGivenInfluence()
    {
      Assert.AreEqual(_influence, SUT.Influences.Single());
    }

    private void ThenItHasAnEmptyListOfInfluences()
    {
      Assert.IsEmpty(SUT.Influences);
    }
  }
}
