#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.ShiftSystems
{
  public interface IShiftPlanAware
  {
    bool IsShiftPlanDependent { get; }

    void OnShiftStarted(ShiftConfiguration shift);

    void OnShiftEnded(ShiftConfiguration shift);
  }
}