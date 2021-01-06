using NUnit.Framework;

namespace GeWISSEN.Utils.Tests
{
  [TestFixture]
  public class MapTests
  {
    public Map<string, string> SUT { get; private set; }
    public string TheResult { get; private set; }

    [Test]
    public void When_given_a_fallback_value_during_ctor_it_returns_this_value_for_unknown_keys()
    {
      const string fallbackValue = "fallback";
      const string invalidKey= "invalidKey";
      
      WhenSutIsCreatedWithFallbackValue(fallbackValue);

      WhenItIsQueriedFor(invalidKey);

      ThenTheResultIs(fallbackValue);
    }

    [Test]
    public void When_given_no_fallback_value_during_ctor_it_returns_the_default_value_for_unknown_keys()
    {
      const string invalidKey = "invalidKey";

      WhenSutIsCreatedWithoutFallbackValue();

      WhenItIsQueriedFor(invalidKey);

      ThenTheResultIs(null);
    }

    [Test]
    public void When_given_a_value_it_returns_this_value_for_the_related_key()
    {
      const string key = "key";
      const string value = "value";

      WhenSutIsCreatedWithoutFallbackValue();

      SUT.Add(key, value);

      WhenItIsQueriedFor(key);

      ThenTheResultIs(value);
    }



    private void WhenItIsQueriedFor(string key)
    {
      TheResult = SUT[key];
    }

    private void WhenSutIsCreatedWithFallbackValue(string fallbackValue)
    {
      SUT = new Map<string, string>(fallbackValue);
    }
    private void WhenSutIsCreatedWithoutFallbackValue()
    {
      SUT = new Map<string, string>();
    }

    private void ThenTheResultIs(string fallbackValue)
    {
      Assert.AreEqual(fallbackValue, TheResult);
    }
  }
}
