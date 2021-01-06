#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;

namespace Milan.Simulation.ShiftSystems
{
  public interface IShiftManagement : IEntity
  {
    IEnumerable<ShiftConfiguration> Shifts { get; }
    IEnumerable<DateTime> Holidays { get; }
    void AddHoliday(DateTime day);
    void RemoveHoliday(DateTime day);
    void AddShift(ShiftConfiguration shift);
    void RemoveShift(ShiftConfiguration shift);
  }
}