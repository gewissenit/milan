#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.ShiftSystems
{
  public interface IWorkingTimeDependent
  {
    bool IsWorkingTimeDependent { get; set; }

    void OnWorkingTimeStarted();

    void OnWorkingTimeEnded();
  }
}