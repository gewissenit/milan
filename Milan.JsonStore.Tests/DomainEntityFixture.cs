#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using NUnit.Framework;

namespace Milan.JsonStore.Tests
{
  /// <summary>
  ///   Base class for domain entities.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class DomainEntityFixture<T>
    where T : DomainEntity
  {
    /// <summary>
    ///   Creates the SUT.
    /// </summary>
    /// <returns></returns>
    protected abstract T CreateSUT();


    /// <summary>
    ///   Creates a minimal configured SUT.
    /// </summary>
    /// <returns></returns>
    protected virtual T CreateMinimalConfiguredSUT()
    {
      var sut = CreateSUT();
      return sut;
    }

    public void SetProperty<TValue>(string propertyName,
                                    TValue value,
                                    Action<TValue, TValue> assertion,
                                    Action<T, TValue> setter,
                                    Func<T, TValue> getter)
    {
      var sut = CreateMinimalConfiguredSUT();
      SetProperty(sut, propertyName, value, assertion, setter, getter);
    }


    public void SetProperty<TValue>(T sut,
                                    string propertyName,
                                    TValue value,
                                    Action<TValue, TValue> assertion,
                                    Action<T, TValue> setter,
                                    Func<T, TValue> getter)
    {
      var eventRaised = false;
      sut.PropertyChanged += (s, e) =>
                             {
                               Assert.AreEqual(sut, s);
                               if (propertyName == e.PropertyName)
                               {
                                 eventRaised = true;
                               }
                             };
      setter(sut, value);
      assertion(value, getter(sut));
      Assert.True(eventRaised);
    }


    public void SetPropertyTwice<TValue>(T sut, TValue value, Action<T, TValue> setter)
    {
      setter(sut, value);
      sut.PropertyChanged += (s, e) => Assert.Fail();
      setter(sut, value);
    }
  }
}