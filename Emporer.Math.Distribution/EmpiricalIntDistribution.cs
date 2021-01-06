#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Empirical distributed stream of pseudo random numbers of type integer.
  ///   Values produced by this distribution follow an empirical distribution which is specified by
  ///   entries consisting of the observed value and the frequency (probability) this value has been observed to occur.
  ///   These entries are made by using the addEntry() method.
  ///   There are a few conditions a user has to meet before actually being allowed to take a Sample of this distribution.
  /// </summary>
  public class EmpiricalIntDistribution : Distribution, IEmpiricalIntDistribution, IRealDistribution
  {
    #region Construtor (s)

    /// <summary>
    ///   Initializes a new instance of the <see cref="EmpiricalIntDistribution" /> class,
    ///   an empirical distribution producing integer values.
    ///   Empirical distributions have to be initialized manually before use.
    ///   This is done by calling the addEntry(integer, double) method to add
    ///   values defining the behaviour of the desired distribution.
    /// </summary>
    public EmpiricalIntDistribution()
    {
      IsValid = IsInitialized;
    }

    #endregion

    #region Values

    /// <summary>
    ///   List to store the entries of Value/cumulative frequency pairs.
    /// </summary>
    private List<EmpiricalIntEntry> _Values;

    internal List<EmpiricalIntEntry> Values
    {
      get
      {
        if (_Values == null)
        {
          _Values = new List<EmpiricalIntEntry>();
        }
        return _Values;
      }
    }

    #endregion

    #region this

    /// <summary>
    ///   Gets the <see cref="EmpiricalIntEntry" /> at the specified index.
    /// </summary>
    /// <value>Record of the values found in index.</value>
    public EmpiricalIntEntry this[int index]
    {
      get { return Values[index]; }
    }

    #endregion

    #region IsInitialized

    private bool _IsInitialized;

    /// <summary>
    ///   Determines whether this IntDistEmpirical distribution already is initialized.
    ///   Being initialized means that all values needed have already been added via the addEntry(integer, double) method.
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

    #region Count

    /// <summary>
    ///   Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
      get { return Values.Count; }
    }

    #endregion

    #region AddEntry

    /// <summary>
    ///   Adds a new entry of an empirical value and its associated cumulative frequency.
    ///   There are restrictions on what will be accepted as a valid entry. If entries do
    ///   not apply to the following list of restrictions, a note will be issued to the user
    ///   1.The lowest Entry in frequency has to be added first.
    ///   2.The last value inserted has to have a cumulative frequency of 1.0 to indicate the upper boundary.
    ///   3.Values have to be added in ascending order of their values and cumulative frequency.
    ///   Any values added with lower value/cumulative frequency than the value added before will Result in a warning message
    ///   and be ignored.
    ///   4.There have to be at least two entries. Otherwise it would be impossible to interpolate a reasonable Sample value.
    ///   5.Once this distribution is "initialized", this method does not accept any additional values to be added.
    ///   6.No two values may be equal, since there can not be two different frequencies for one observed value.
    ///   Entries with same value/frequency pair as an
    ///   entry already made before are simply ignored.
    ///   Only if all conditions described above apply and this distribution is "initialized",
    ///   it is possible to obtain Samples via the Sample() method.
    /// </summary>
    /// <param name="value">The empirical value observed.</param>
    /// <param name="frequency">The corresponding cumulative frequency of the empirical value.</param>
    /// <returns>true, if entry could be done, false if not.</returns>
    public bool TryAddEntry(int value, double frequency)
    {
      if (!IsInitialized)
      {
        // frequency must be positive
        if (((frequency < 0) || (frequency > 1.0)))
        {
          return false;
        }
        // frequency of 1.0 indicates last element and initialization of distribution
        if ((frequency == 1.0))
        {
          if (Values.Count == 0)
          {
            Values.Add(new EmpiricalIntEntry()
                       {
                         Value = value,
                         Frequency = frequency
                       });
            IsInitialized = true;
            return true;
          }
          if (Values[Values.Count - 1].Value < value)
          {
            Values.Add(new EmpiricalIntEntry()
                       {
                         Value = value,
                         Frequency = frequency
                       });
            IsInitialized = true;
            return true;
          }
          return false;
        }
        // always check for invalid values/frequencies
        for (var i = 0; i < Values.Count; i++)
        {
          if (((Values[i].Value >= value) || (Values[i].Frequency >= frequency)))
          {
            return false;
          }
        }
        Values.Add(new EmpiricalIntEntry()
                   {
                     Value = value,
                     Frequency = frequency
                   });
        return true;
      }
      return false;
    }

    #endregion

    #region Sample

    /// <summary>
    ///   Returns the next Sample specified by the empirical distribution. In contrast to RealDistEmpirical here is no
    ///   interpolation needed.
    /// </summary>
    /// <returns>
    ///   The next Sample for this empirical distribution or returns zero (0) with a warning
    ///   if the distribution has not been properly initialized yet
    /// </returns>
    int IIntDistribution.GetSample()
    {
      if (IsValid)
      {
        var result = 0;
        double q = RandomGenerator.NextDouble();
        for (var i = 0; i < Values.Count; i++)
        {
          if (Values[i].Frequency >= q)
          {
            result = Values[i].Value;
            break;
          }
        }
        return result;
      }
      throw new InvalidOperationException(Validate());
    }

    double IRealDistribution.GetSample()
    {
      return Convert.ToDouble((this as IIntDistribution).GetSample());
    }

    #endregion

    #region ValidateCore

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (!IsInitialized)
      {
        messages.Append("EmpiricalDistribution have no entries and so it is not initialized. Use AddEntry-Method to add entries into the list.");
      }
      return messages;
    }

    #endregion

    #region Reset

    public override void Reset()
    {
      base.Reset();
      _Values = null;
      IsInitialized = false;
    }

    #endregion

    #region Entries

    public ICollection<EmpiricalIntEntry> Entries
    {
      get { return Values; }
    }

    #endregion

    // TODO: refactoring of this class! some ideas, remove AddEntry and let set/get entries as a list of entries over a property (ICollection<EmpiricalIntEntry>)
    // TODO: To make sure that all restrictions will be issued, check it in an own List-type for empirical values in the add-method.
  }
}