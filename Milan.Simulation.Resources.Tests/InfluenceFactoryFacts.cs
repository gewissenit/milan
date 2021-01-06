using Milan.Simulation.Resources.Factories;
using NUnit.Framework;

namespace Milan.Simulation.Resources.Tests
{
  [TestFixture]
  public class InfluenceFactoryFacts
  {
    private IInfluence _result;
    private Influence _givenInstance;

    private IInfluenceFactory SUT { get; set; }

    
    public void It_creates_influence_instances()
    {
      WhenSutIsCreated();

      WhenItCreatesAnInstance();

      ThenTheResultIsNotNull();
    }

    
    public void It_duplicates_influence_instances()
    {
      const string someName = "some name";
      GivenAnotherInstance(someName);
      WhenSutIsCreated();

      WhenItDuplicatesAGivenInstance();

      ThenTheResultIsEqualToTheGivenInstance(someName);

    }

    private void GivenAnotherInstance(string someName)
    {
      _givenInstance = new Influence();
      _givenInstance.Name = someName;
    }

    private void WhenSutIsCreated()
    {
      //SUT = new InfluenceFactory();
    }

    private void WhenItCreatesAnInstance()
    {
      _result = SUT.Create();
    }

    private void WhenItDuplicatesAGivenInstance()
    {
      _result = SUT.Duplicate(_givenInstance);
    }

    private void ThenTheResultIsNotNull()
    {
      Assert.NotNull(_result);
    }

    private void ThenTheResultIsEqualToTheGivenInstance(string someName)
    {
      Assert.AreEqual(someName, _result.Name);
    }
  }
}
