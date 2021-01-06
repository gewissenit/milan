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
  ///   Empirical distributed stream of pseudo random numbers of type double. Values produced by
  ///   this distribution follow an empirical distribution which is specified by entries consisting
  ///   of the observed value and the frequency (probability) this value has been observed to occur. These entries are made
  ///   by using the addEntry()
  ///   method. There are a few conditions a user has to meet before actually being allowed to take a Sample of this
  ///   distribution.
  /// </summary>
  public class EmpiricalRealDistribution : Distribution, IEmpiricalRealDistribution
  {
    #region Constructor(s)

    /// <summary>
    ///   Initializes a new instance of the <see cref="EmpiricalRealDistribution" /> class, an empirical distribution pruducing
    ///   floating point values. Empirical distributions have to be initialized manually before use.
    ///   This is done by calling the addEntry(double, double) method to add values defining the behaviour of the desired
    ///   distribution.
    /// </summary>
    public EmpiricalRealDistribution()
    {
      IsValid = IsInitialized;
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

    #region Entries

    /// <summary>
    ///   List to store the entries of Value/cumulative frequency pairs.
    /// </summary>

    #region Entries
    public ICollection<EmpiricalRealEntry> Entries
    {
      get { return Values; }
    }

    #endregion

    #region Values

    /// <summary>
    ///   List to store the entries of Value/cumulative frequency pairs.
    /// </summary>
    private List<EmpiricalRealEntry> _Values;

    internal List<EmpiricalRealEntry> Values
    {
      get
      {
        if (_Values == null)
        {
          _Values = new List<EmpiricalRealEntry>();
        }
        return _Values;
      }
    }

    #endregion

    #region AddEntry

    /// <summary>
    ///   Adds a new entry of an empirical value and its associated cumulative frequency.
    ///   There are restrictions on what will be accepted as a valid entry. If entries do
    ///   not apply to the following list of restrictions, a note will be issued to the user
    ///   1.The first value inserted has to have a cumulative frequency of 0.0 to indicate the lower
    ///   boundary of the distribution, the last value inserted has to have a cumulative frequency of 1.0 to indicate the upper
    ///   boundary.
    ///   2.Values have to be added in ascending order of their values and cumulative frequency.
    ///   Any values added with lower value/cumulative frequency than the
    ///   value added before will Result in a warning message.
    ///   3.There have to be at least two entries. Otherwise it would be impossible to interpolate or give a reasonable Sample
    ///   value.
    ///   4.Once this distribution is "initialized" via get the first sample, this method does not accept any more values to be
    ///   added.
    ///   5.No two values may be equal, since there can not be two diferent frequencies for one observed value.
    ///   Entries with same value/frequency pair as an entry
    ///   already made before are simply ignored.
    ///   Only if all conditions described above apply,
    ///   it is possible to obtain Samples via the Sample() method.
    /// </summary>
    /// <param name="value">The empirical value observed.</param>
    /// <param name="frequency">true, if entry could be done, false if not.</param>
    /// <returns></returns>
    public bool TryAddEntry(double value, double frequency)
    {
      if (!IsInitialized)
      {
        // frequency must be between 0 and 1.
        if (((frequency < 0) || (frequency > 1.0)))
        {
          return false;
        }
        if (frequency == 1.0)
        {
          if (Values.Count == 0)
          {
            Values.Add(new EmpiricalRealEntry()
                       {
                         Value = value,
                         Frequency = frequency
                       });
            IsInitialized = true;
            return true;
          }
          if (Values[Values.Count - 1].Value < value)
          {
            Values.Add(new EmpiricalRealEntry()
                       {
                         Value = value,
                         Frequency = frequency
                       });
            IsInitialized = true;
            return true;
          }
          return false;
        }
        for (var i = 0; i < Values.Count; i++)
        {
          if (((Values[i].Value >= value) || (Values[i].Frequency >= frequency)))
          {
            return false;
          }
        }
        Values.Add(new EmpiricalRealEntry()
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
    ///   Returns the next Sample specified by the empirical distribution. Return values are calculated by linear interpolation
    ///   between the two values surrounding that value given by the random generator.
    /// </summary>
    /// <returns>The next Sample for this empirical distribution or 0 if the distribution has not been properly initialized yet</returns>
    double IRealDistribution.GetSample()
    {
      // must not already be initialized
      if (IsValid)
      {
        double result = 0;
        double q = RandomGenerator.NextDouble();
        for (var i = 1; i < Values.Count; i++)
        {
          if (Values[i].Frequency >= q)
          {
            double lowValue = Values[i - 1].Value;
            double lowFreq = Values[i - 1].Frequency;
            double highValue = Values[i].Value;
            double highFreq = Values[i].Frequency;
            result = lowValue + (((highValue - lowValue) * (q - lowFreq)) / (highFreq - lowFreq));
            break;
          }
        }
        return result;
      }
      throw new InvalidOperationException(Validate());
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

    #endregion

    // TODO: It doesn't work because at this line: _Values = new EmpiricalRealEntry[_Values.Length + 1]; we lost all existing values before.
    // TODO: refactoring of this class! some ideas, remove AddEntry and let set/get entries as a list of entries over a property (ICollection<EmpiricalIntEntry>)
    // TODO: To make sure that all restrictions will be issued, check it in an own List-type for empirical values in the add-method.
  }
}