#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using NUnit.Framework;

namespace Milan.Simulation.Tests
{
// ReSharper disable InconsistentNaming
  [TestFixture]
  public class SimulationTimeConversionFixture
  {
    private const double One = 1d;
    private const double OneThousand = 1000d;
    private const double SixtyThousand = 60000d;
    private const double ThreeMillionSixHundredThousand = 3600000d;
    private const double EightySixMillionFourHundredThousand = 86400000;
    private const double SixHundredFourMillionEightHundredThousand = 604800000d;

    private readonly TimeSpan MiliSecond = TimeSpan.FromMilliseconds(1);
    private readonly TimeSpan Second = TimeSpan.FromSeconds(1);
    private readonly TimeSpan Minute = TimeSpan.FromMinutes(1);
    private readonly TimeSpan Hour = TimeSpan.FromHours(1);
    private readonly TimeSpan Day = TimeSpan.FromDays(1);
    private readonly TimeSpan Week = TimeSpan.FromDays(7);

    [Test]
    public void ToRealTimeSpan___Converts_Typical_Values_To_Real_Time_Spans()
    {
      Assert.AreEqual(MiliSecond, One.ToRealTimeSpan());
      Assert.AreEqual(Second, OneThousand.ToRealTimeSpan());
      Assert.AreEqual(Minute, SixtyThousand.ToRealTimeSpan());
      Assert.AreEqual(Hour, ThreeMillionSixHundredThousand.ToRealTimeSpan());
      Assert.AreEqual(Day, EightySixMillionFourHundredThousand.ToRealTimeSpan());
      Assert.AreEqual(Week, SixHundredFourMillionEightHundredThousand.ToRealTimeSpan());
    }

    [Test]
    public void ToSimulationTimeSpan___Converts_Typical_Values_To_Simulation_Time_Spans()
    {
      Assert.AreEqual(One, MiliSecond.ToSimulationTimeSpan());
      Assert.AreEqual(OneThousand, Second.ToSimulationTimeSpan());
      Assert.AreEqual(SixtyThousand, Minute.ToSimulationTimeSpan());
      Assert.AreEqual(ThreeMillionSixHundredThousand, Hour.ToSimulationTimeSpan());
      Assert.AreEqual(EightySixMillionFourHundredThousand, Day.ToSimulationTimeSpan());
      Assert.AreEqual(SixHundredFourMillionEightHundredThousand, Week.ToSimulationTimeSpan());
    }
  }

// ReSharper restore InconsistentNaming
}