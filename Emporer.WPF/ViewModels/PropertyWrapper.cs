#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Caliburn.Micro;

namespace Emporer.WPF.ViewModels
{
  /// <summary>
  ///   A proxy object that wraps a property of another object. This can be used in UI bindings that can not operate on the
  ///   proxied reference directly.
  /// </summary>
  /// <remarks>
  ///   WPF DataTemplates can not change directly bound objects. They need a property to which they can bind via
  ///   Path=[PropertyName].
  /// </remarks>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  public class PropertyWrapper<TValue> : PropertyChangedBase
  {
    private readonly Func<TValue> _getValue;
    private readonly Action<TValue> _setValue;

    /// <summary>
    ///   Initializes a new instance of the <see cref="PropertyWrapper&lt;TValue&gt;" /> class.
    /// </summary>
    /// <param name="getAccessor">A delegate calling the get accessor of the wrapped property.</param>
    /// <param name="setAccessor">A delegate calling the set accessor of the wrapped property.</param>
    public PropertyWrapper(Func<TValue> getAccessor, Action<TValue> setAccessor)
    {
      _getValue = getAccessor;
      _setValue = setAccessor;
    }


    /// <summary>
    ///   Gets or sets the value of the wrapped property.
    /// </summary>
    /// <value>
    ///   The value.
    /// </value>
    public TValue Value
    {
      get { return _getValue(); }
      set
      {
        _setValue(value);
        NotifyOfPropertyChange();
      }
    }
  }
}