#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Empirical distributed stream of pseudo random numbers of type double.
  ///   Values produced by this distribution follow an empirical distribution which is specified by
  ///   entries consisting of the observed value and the frequency (probability) this value has been observed to occur.
  ///   There are a few conditions a user has to meet before actually being allowed to take a Sample of this distribution.
  ///   Note that this interface will be extend and perhaps change for the future.
  /// </summary>
  public interface IEmpiricalRealDistribution : IRealDistribution
  {
    ICollection<EmpiricalRealEntry> Entries { get; }

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
    /// <param name="frequency">.</param>
    bool TryAddEntry(double value, double frequency);
  }
}