#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation
{
  /// <summary>
  ///   Interface for any time managing objects.
  /// </summary>
  public interface IClock : ITimeProvider
  {
    /// <summary>
    ///   The smallest distinguishable amount of simulation time.
    /// </summary>
    double Epsilon { get; }

    /// <summary>
    ///   Advances the current time by the given time difference.
    /// </summary>
    /// <param name="timeDifference">The amount of time added to the current time of the clock.</param>
    void AdvanceTime(double timeDifference);

    /// <summary>
    ///   Resets the clock and sets start time and current time to the given value.
    /// </summary>
    /// <param name="startTime">The point in time the clock should start with.</param>
    void Reset(double startTime);
  }
}