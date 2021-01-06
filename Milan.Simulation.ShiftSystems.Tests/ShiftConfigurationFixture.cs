#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.JsonStore.Tests;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Milan.Simulation.ShiftSystems.Tests
{
  [TestFixture]
  public class ShiftConfigurationFixture: DomainEntityFixture<ShiftConfiguration>
  {
    [Test]
    public void Set_Duration()
    {
      var value = TimeSpan.FromHours(8);
      SetProperty("Duration", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Duration = v, s => s.Duration);
    }
    
    [Test]
    public void Set_Same_Duration_Does_Not_Raise_PropertyChanged()
    {
      var value = TimeSpan.FromHours(8);
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Duration = v);
    }

    [Test]
    public void Set_StartTime()
    {
      var value = DateTime.Now;
      SetProperty("StartTime", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.StartTime = v, s => s.StartTime);
    }


    [Test]
    public void Set_Same_StartTime_Does_Not_Raise_PropertyChanged()
    {
      var value = DateTime.Now;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.StartTime = v);
    }

    [Test]
    public void Set_StartDay()
    {
      var value = DayOfWeek.Monday;
      SetProperty("StartDay", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.StartDay = v, s => s.StartDay);
    }


    [Test]
    public void Set_Same_StartDay_Does_Not_Raise_PropertyChanged()
    {
      var value = DayOfWeek.Monday;
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.StartDay = v);
    }
    
    protected override ShiftConfiguration CreateSUT()
    {
      return new ShiftConfiguration();
    }
  }
}

// ReSharper restore InconsistentNaming