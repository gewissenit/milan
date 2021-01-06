using System.Collections.Generic;
using NUnit.Framework;
using Ork.Framework.Services;

namespace Ork.Framework.Tests
{
  [TestFixture]
  public class KeyBindingServiceFacts
  {
    private IKeyBindingService SUT { get; set; }

    public object GivenSubscriber { get; set; }
    private bool TheGivenActionWasCalled { get; set; }

    private static IEnumerable<string[]> TestKeySequences
    {
      get
      {
        yield return new[]
                                        {
                                          "Alt",
                                          "Enter"
                                        };
        yield return new[] { "Enter",
                                          "Alt"
                                        };
      }
    }

    [Test]
    public void It_can_be_created()
    {
      WhenSutIsCreated();
    }

    [Test]
    public void It_can_receive_pressed_keys()
    {
      const string pressedKey = "Enter";
      const string sender = "sender";

      WhenSutIsCreated();

      SUT.ReceiveKeyPressed(pressedKey, sender);
    }

    [Test]
    public void It_can_received_released_keys()
    {
      const string releasedKey = "A";
      const string sender = "sender";

      WhenSutIsCreated();

      SUT.ReceiveKeyReleased(releasedKey, sender);
    }

    [Test]
    public void Clients_can_subscribe_to_key_sequences()
    {
      WhenSutIsCreated();

      GivenSubscriber = new object();

      WhenTheGivenSubscriberSuscribedAnActionFor("Alt+Enter");
    }

    [Test]
    [TestCaseSource("TestKeySequences")]
    public void When_a_client_is_subscribed_to_a_key_sequence_it_receives_a_notification_when_the_sequence_is_pressed(
      string[] sequence)
    {
      GivenSubscriber = new object();
      WhenSutIsCreated();

      WhenTheGivenSubscriberSuscribedAnActionFor("Alt+Enter");

      WhenASequenceOfKeyIsPressed(sequence);

      ThenTheGivenActionWasCalled();
    }

    private void WhenTheGivenSubscriberSuscribedAnActionFor(string keySequence)
    {
      SUT.SubscribeFor(keySequence, GivenSubscriber, () => { TheGivenActionWasCalled = true; });
    }

    private void WhenASequenceOfKeyIsPressed(IEnumerable<string> sequence)
    {
      foreach (var key in sequence)
      {
        SUT.ReceiveKeyPressed(key, "");
      }
    }

    private void ThenTheGivenActionWasCalled()
    {
      Assert.IsTrue(TheGivenActionWasCalled);
    }

    private void WhenSutIsCreated()
    {
      SUT = new KeyBindingService();
    }
  }
}