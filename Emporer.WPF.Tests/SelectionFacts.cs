using System;
using NUnit.Framework;

namespace Emporer.WPF.Tests
{
  [TestFixture]
  public class SelectionFacts
  {
    [SetUp]
    public void Setup()
    {
      ExpectedSelection = null;
    }

    private ISelection SUT { get; set; }
    private object SomeSubscriber { get; set; }
    private object ExpectedSelection { get; set; }
    private Action<object> SomeSubscription { get; set; }
    private object SomeSelectionThatIsMade { get; set; }
    private A ObjectSentToSubscribers { get; set; }
    private Action<A> GivenSubscription { get; set; }

    [Test]
    public void It_depends_on_nothing()
    {
      WhenSutIsCreated();
    }

    [Test]
    public void It_does_not_notify_subscribers_when_a_selection_was_made_by_themselves()
    {
      WhenSutIsCreated();
      GivenASubscriber();
      GivenASubscription();
      WhenASubscriberSubscribed();
      SomeSelectionThatIsMade = new object();

      WhenANewSelectionIsMadeByTheSubscriber();

      ThenTheSubscriberWasNotNotifiedOfTheNewSelection();
    }

    [Test]
    public void It_lets_clients_subscribe_to_selection_changes()
    {
      WhenSutIsCreated();
      GivenASubscriber();
      GivenASubscription();
      WhenASubscriberSubscribed();
    }

    [Test]
    public void It_notifies_subscribers_when_a_selection_was_made()
    {
      WhenSutIsCreated();
      GivenASubscriber();
      GivenASubscription();
      WhenASubscriberSubscribed();
      SomeSelectionThatIsMade = new object();

      WhenANewSelectionIsMadeNotByTheSubscriber();

      ThenTheSubscriberWasNotifiedOfTheNewSelection();
    }

    [Test]
    public void It_notifies_subscribers_when_a_selection_was_made_that_is_of_a_derived_type_of_the_subscription()
    {
      WhenSutIsCreated();
      GivenASubscriber();
      GivenSubscription = (A a) =>
                                {
                                  ObjectSentToSubscribers = a;
                                };
      SUT.Subscribe(SomeSubscriber, GivenSubscription);
      var aSelectionThatIsMade = new ADerived();

      SUT.Select(aSelectionThatIsMade, new object());

      Assert.NotNull(ObjectSentToSubscribers);
      Assert.AreSame(ObjectSentToSubscribers, aSelectionThatIsMade);
    }

    [Test]
    public void Subscribers_can_unsubscribe()
    {
      WhenSutIsCreated();
      GivenASubscriber();
      GivenASubscription();
      WhenASubscriberSubscribed();
      SomeSelectionThatIsMade = new object();

      SUT.Unsubscribe<object>(SomeSubscriber);

      WhenANewSelectionIsMadeNotByTheSubscriber();

      ThenTheSubscriberWasNotNotifiedOfTheNewSelection();
    }

    [Test]
    public void Subscribers_can_unsubscribe_for_specific_subscriptions()
    {
      
    }


    [Test]
    public void It_can_select_null()
    {
      WhenSutIsCreated();
      SUT.Select<object>(null, null);
    }

    private void WhenSutIsCreated()
    {
      SUT = new Selection();
    }

    private void ThenTheSubscriberWasNotifiedOfTheNewSelection()
    {
      Assert.AreSame(ExpectedSelection, SomeSelectionThatIsMade);
    }

    private void WhenANewSelectionIsMadeNotByTheSubscriber()
    {
      SUT.Select(SomeSelectionThatIsMade, this);
    }

    private void WhenANewSelectionIsMadeByTheSubscriber()
    {
      SUT.Select(SomeSelectionThatIsMade, SomeSubscriber);
    }

    private void WhenASubscriberSubscribed()
    {
      SUT.Subscribe(SomeSubscriber, SomeSubscription);
    }

    private void GivenASubscription()
    {
      SomeSubscription = (o) =>
                       {
                         ExpectedSelection = o;
                       };
    }

    private void GivenASubscriber()
    {
      SomeSubscriber = new object();
    }

    private void ThenTheSubscriberWasNotNotifiedOfTheNewSelection()
    {
      Assert.Null(ExpectedSelection);
    }

    private class A
    {
    }

    private class ADerived : A
    {
    }
  }
}