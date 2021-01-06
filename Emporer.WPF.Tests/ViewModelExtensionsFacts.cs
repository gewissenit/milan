using System;
using System.ComponentModel;
using Caliburn.Micro;
using NUnit.Framework;

namespace Emporer.WPF.Tests
{
  [TestFixture]
  public class ViewModelExtensionsFacts
  {
    //technically, this is not the SUT, but the extension methods in ViewModelExtensions work on it
    private SomeViewModel _sut;
    private const int Once = 1;
    private const int Twice = 2;

    [Test]
    public void SetAndRaiseIfChanged___changes_the_specified__value_type_field_when_the_value_is_different()
    {
      const int someValue = 42;

      WhenItIsCreated();

      WhenItsValueTypePropertyIsSetTo(someValue);

      ThenItsValueTypePropertyIs(someValue);
    }

    [Test]
    public void SetAndRaiseIfChanged___changes_the_specified_reference_type_field_when_the_value_is_different()
    {
      var someValue = new SomeViewModel();

      WhenItIsCreated();

      WhenItsReferenceTypePropertyIsSetTo(someValue);

      ThenItsReferenceTypePropertyIs(someValue);
    }
    
    [Test]
    public void SetAndRaiseIfChanged___raises_PropertyChanged_when_it_changed_the_value()
    {
      WhenItIsCreated();
      var timesTheEventWasRaised = 0;
      _sut.PropertyChanged += (s, e) => timesTheEventWasRaised++;

      WhenItsValueTypePropertyIsSetTo(42);
      Assert.AreEqual(Once, timesTheEventWasRaised);

      WhenItsValueTypePropertyIsSetTo(42);
      Assert.AreEqual(Once, timesTheEventWasRaised);

      WhenItsValueTypePropertyIsSetTo(24);
      Assert.AreEqual(Twice, timesTheEventWasRaised);
    }
    
    private void WhenItIsCreated()
    {
      _sut = new SomeViewModel();
    }

    private void WhenItsValueTypePropertyIsSetTo(int someValue)
    {
      _sut.ValueTypeProperty = someValue;
    }

    private void WhenItsReferenceTypePropertyIsSetTo(SomeViewModel someValue)
    {
      _sut.ReferenceTypeProperty = someValue;
    }

    private void ThenItsValueTypePropertyIs(int someValue)
    {
      Assert.AreEqual(someValue, _sut.ValueTypeProperty);
    }

    private void ThenItsReferenceTypePropertyIs(INotifyPropertyChanged someValue)
    {
      Assert.AreSame(someValue, _sut.ReferenceTypeProperty);
    }

    private class SomeViewModel : PropertyChangedBase
    {
      private SomeViewModel _referenceTypeProperty;
      private int _valueTypeProperty;

      public int ValueTypeProperty
      {
        get { return _valueTypeProperty; }
        set { this.SetAndRaiseIfChanged(ref _valueTypeProperty, value); }
      }

      public SomeViewModel ReferenceTypeProperty
      {
        get { return _referenceTypeProperty; }
        set { this.SetAndRaiseIfChanged(ref _referenceTypeProperty, value); }
      }
    }
  }
}