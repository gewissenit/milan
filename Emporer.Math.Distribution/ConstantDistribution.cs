#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  public class ConstantDistribution : Distribution, IConstantRealDistribution, IConstantIntDistribution, IConstantBoolDistribution
  {
    #region IsValid

    internal override bool IsValid
    {
      set { base.IsValid = value; }
    }

    #endregion

    #region ConstantValue

    private bool _BooleanConstantValue;
    private double _DoubleConstantValue;

    private int _IntConstantValue;

    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("ConstantValue")]
    [Category("Parameters")]
    [Description("The ConstantValue of this distribution, which will be always returned.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    bool IConstantBoolDistribution.ConstantValue
    {
      get { return _BooleanConstantValue; }
      set
      {
        _BooleanConstantValue = value;
        IsValid = true;
      }
    }

    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("ConstantValue")]
    [Category("Parameters")]
    [Description("The ConstantValue of this distribution, which will be always returned.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    int IConstantIntDistribution.ConstantValue
    {
      get { return _IntConstantValue; }
      set
      {
        _IntConstantValue = value;
        _DoubleConstantValue = Convert.ToDouble(value);
        IsValid = true;
      }
    }

    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("ConstantValue")]
    [Category("Parameters")]
    [Description("The ConstantValue of this distribution, which will be always returned.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    double IConstantRealDistribution.ConstantValue
    {
      get { return _DoubleConstantValue; }
      set
      {
        _DoubleConstantValue = value;
        IsValid = true;
      }
    }

    #endregion

    #region GetSample

    /// <summary>
    ///   Returns the next constant boolean Sample of this distribution.
    ///   For this "pseudo"-distribution it is always is the default value
    ///   specified through the constructor or via the ConstantValue propertie.
    /// </summary>
    /// <returns>The constant Sample.</returns>
    bool IBoolDistribution.GetSample()
    {
      if (IsValid)
      {
        return _BooleanConstantValue;
      }
      throw new InvalidOperationException(Validate());
    }

    int IIntDistribution.GetSample()
    {
      if (IsValid)
      {
        return _IntConstantValue;
      }
      throw new InvalidOperationException(Validate());
    }

    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        return _DoubleConstantValue;
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion
  }
}