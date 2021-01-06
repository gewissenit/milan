#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation.ShiftSystems
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ShiftConfiguration : DomainEntity
  {
    [JsonProperty]
    public DateTime StartTime
    {
      get { return Get<DateTime>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public DayOfWeek StartDay
    {
      get { return Get<DayOfWeek>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public TimeSpan Duration
    {
      get { return Get<TimeSpan>(); }
      set { Set(value); }
    }
  }
}