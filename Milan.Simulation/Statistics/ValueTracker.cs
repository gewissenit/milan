#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class ValueTracker<TValue> : StatisticalCounter<TValue>, IValueTracker<TValue>
  {
    private Func<double, TValue> _fromDouble;
    private TValue _maximum;
    private TValue _minimum;
    private Func<TValue, double> _toDouble;
    private Action _updateInternally;

    public ValueTracker()
    {
      ChooseDefaultConversions();
      _updateInternally = DoFirstUpdate;
    }

    public ValueTracker(Func<TValue, double> toDouble, Func<double, TValue> fromDouble)
      : this()
    {
      _toDouble = toDouble;
      _fromDouble = fromDouble;
    }


    public Func<TValue, double> ToDouble
    {
      get { return _toDouble; }
    }

    public Func<double, TValue> FromDouble
    {
      get { return _fromDouble; }
    }

    public double CurrentValueNumeric
    {
      get { return ToDouble(CurrentValue); }
    }

    public TValue Maximum
    {
      get { return _maximum; }
      protected set
      {
        _maximum = value;
        OnMaximumChanged();
      }
    }


    public TValue Minimum
    {
      get { return _minimum; }
      protected set
      {
        _minimum = value;
        OnMinimumChanged();
      }
    }

    public double MinimumNumeric
    {
      get { return ToDouble(Minimum); }
    }

    public double MaximumNumeric
    {
      get { return ToDouble(Maximum); }
    }


    public override void Update(TValue value)
    {
      base.Update(value);
      _updateInternally();
    }


    /// <summary>
    ///   Does the first update.
    /// </summary>
    protected virtual void DoFirstUpdate()
    {
      Minimum = CurrentValue;
      Maximum = CurrentValue;

      // here we unlink this method itself from the update procedure and set another method that does them from now on
      _updateInternally = DoConsecutiveUpdate;
    }

    /// <summary>
    ///   Does consecutive updates (after the first one was done by <see cref="DoFirstUpdate" />).
    /// </summary>
    protected virtual void DoConsecutiveUpdate()
    {
      var currentValueNumeric = ToDouble(CurrentValue);
      if (MinimumNumeric > currentValueNumeric)
      {
        Minimum = CurrentValue;
      }

      if (MaximumNumeric < currentValueNumeric)
      {
        Maximum = CurrentValue;
      }
    }

    protected virtual void OnMinimumChanged()
    {
    }

    protected virtual void OnMaximumChanged()
    {
    }

    #region default conversion related

    private void ChooseDefaultConversions()
    {
      if (typeof (TValue) == typeof (byte))
      {
        _fromDouble = ToByte;
      }

      if (typeof (TValue) == typeof (short))
      {
        _fromDouble = ToShort;
      }

      if (typeof (TValue) == typeof (int))
      {
        _fromDouble = ToInt;
      }

      if (typeof (TValue) == typeof (long))
      {
        _fromDouble = ToLong;
      }

      if (typeof (TValue) == typeof (float))
      {
        _fromDouble = ToFloat;
      }

      if (typeof (TValue) == typeof (double))
      {
        _fromDouble = DoNothing;
      }

      if (_fromDouble != null) // looks like one of the above conversions was chosen
      {
        _toDouble = AllConvertibleToDouble;
      }
      else // fallback
      {
        _fromDouble = d => default(TValue);
        _toDouble = value => double.NaN;
      }
    }

    private double AllConvertibleToDouble(TValue value)
    {
      double result;
      try
      {
        result = System.Convert.ToDouble(value); //boxing result to be castable
      }
      catch (Exception)
      {
        result = default(long);
      }
      return result;
    }

    private TValue ToByte(double value)
    {
      return Convert(value, System.Convert.ToByte);
    }

    private TValue ToShort(double value)
    {
      return Convert(value, System.Convert.ToInt16);
    }

    private TValue ToInt(double value)
    {
      return Convert(value, System.Convert.ToInt32);
    }

    private TValue ToLong(double value)
    {
      return Convert(value, System.Convert.ToInt64);
    }

    private TValue ToFloat(double value)
    {
      return Convert(value, System.Convert.ToSingle);
    }

    private TValue DoNothing(double value)
    {
      return Convert(value, d => d);
    }

    private TValue Convert<TTargetType>(double value, Func<double, TTargetType> convert)
    {
      object boxed;
      try
      {
        boxed = convert(value); //boxing result to be castable
      }
      catch (Exception)
      {
        boxed = default(long);
      }
      return (TValue) boxed;
    }

    #endregion
  }
}