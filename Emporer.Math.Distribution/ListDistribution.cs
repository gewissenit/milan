#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Emporer.Math.Distribution
{
  public class ListDistribution : Distribution, IListDistribution, IRealDistribution
  {
    #region Count

    private int _Count;

    internal int Count
    {
      get { return _Count; }
      set { _Count = value; }
    }

    #endregion
    
    #region Values

    private List<double> _Values;

    public List<double> Values
    {
      get
      {
        if (_Values == null)
        {
          _Values = new List<double>();
        }
        return _Values;
      }
    }

    #endregion

    #region IsValid

    private new bool _IsValid;

    internal override bool IsValid
    {
      get { return _IsValid; }
      set { _IsValid = value; }
    }

    #endregion

    #region EntryCount

    /// <summary>
    ///   Gets the entry count.
    /// </summary>
    /// <value>The entry count.</value>
    public int EntryCount
    {
      get { return Values.Count; }
    }

    #endregion

    #region AddEntry

    /// <summary>
    ///   Adds the entry.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public void AddEntry(double value)
    {
      //todo: this is not good for property editor. rethink, or find a way to add entries via property grid
      Values.Add(value);
      IsInitialized = true;
    }

    #endregion

    #region IsInitialized

    private bool _IsInitialized;

    /// <summary>
    ///   Determines whether this empirical distribution has been properly initialized.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </returns>
    public bool IsInitialized
    {
      get { return _IsInitialized; }
      private set
      {
        _IsInitialized = value;
        IsValid = value;
      }
    }

    #endregion

    #region IsPeriodic

    private bool _IsPeriodic;

    /// <summary>
    ///   Gets or sets a value indicating whether this distribution is periodic.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this distribution is periodic; otherwise, <c>false</c>.
    /// </value>
    public bool IsPeriodic
    {
      get { return _IsPeriodic; }
      set { _IsPeriodic = value; }
    }

    #endregion

    #region Sample (override)

    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        if ((Count > (Values.Count - 1)) &&
            !_IsPeriodic)
        {
          var msg = string.Format(CultureInfo.InvariantCulture,
                                  Constants.ErrorDescSample + Constants.ErrorReasonUndersizedList + Constants.ErrorPrevUndersizedList);
          throw new InvalidOperationException(msg);
        }
        var value = Values[Count % Values.Count];
        Count++;
        return value;
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    #region ValidateCore

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (!IsInitialized)
      {
        messages.Append("ListDistribution have no entries and so it is not initialized. Use AddEntry-Method to add entries into the list.");
      }
      return messages;
    }

    #endregion

    #region Reset

    /// <summary>
    ///   Resets the pseudo random generator's seed and the number of Samples given to zero
    ///   and sets antithetic to false and NonNegative to true for this distribution.
    /// </summary>
    public override void Reset()
    {
      base.Reset();
      _Values = null;
      Count = 0;
      IsValid = false;
    }

    #endregion
  }
}