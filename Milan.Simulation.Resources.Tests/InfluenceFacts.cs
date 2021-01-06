using NUnit.Framework;

namespace Milan.Simulation.Resources.Tests
{
  [TestFixture]
  public class InfluenceFacts
  {
    public IInfluence SUT { get; private set; }

    [Test]
    public void It_has_a_name()
    {
      WhenSutIsCreated();

      ThenItsNameIsAnEmptyString();
    }
    
    private void ThenItsNameIsAnEmptyString()
    {
      Assert.AreEqual(string.Empty, SUT.Name);
    }

    private void WhenSutIsCreated()
    {
      SUT = new Influence();
    }

  }
}
