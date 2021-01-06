#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Milan.Simulation.ShiftSystems.Tests
{
  [TestFixture]
  public class UtilsFixture
  {
    // shorten enum usages 
    private const DayOfWeek Monday = DayOfWeek.Monday;
    private const DayOfWeek Tuesday = DayOfWeek.Tuesday;
    private const DayOfWeek Wednesday = DayOfWeek.Wednesday;
    private const DayOfWeek Thursday = DayOfWeek.Thursday;
    private const DayOfWeek Friday = DayOfWeek.Friday;
    private const DayOfWeek Saturday = DayOfWeek.Saturday;
    private const DayOfWeek Sunday = DayOfWeek.Sunday;


    // two weeks of specific test dates
    private readonly DateTime Monday_2012_05_21 = new DateTime(2012, 05, 21);

    private readonly DateTime Tuesday_2012_05_22 = new DateTime(2012, 05, 22);

    //private readonly DateTime Wednesday_2012_05_23 = new DateTime(2012, 05, 23);
    //private readonly DateTime Thursday_2012_05_24 = new DateTime(2012, 05, 24);
    //private readonly DateTime Friday_2012_05_25 = new DateTime(2012, 05, 25);
    private readonly DateTime Saturday_2012_05_26 = new DateTime(2012, 05, 26);

    private readonly DateTime Sunday_2012_05_27 = new DateTime(2012, 05, 27);

    private readonly DateTime Monday_2012_05_28 = new DateTime(2012, 05, 28);

    private readonly DateTime Tuesday_2012_05_29 = new DateTime(2012, 05, 29);

    //private readonly DateTime Wednesday_2012_05_30 = new DateTime(2012, 05, 30);
    //private readonly DateTime Thursday_2012_05_31 = new DateTime(2012, 05, 31);
    //private readonly DateTime Friday_2012_06_01 = new DateTime(2012, 06, 1);
    private readonly DateTime Saturday_2012_06_02 = new DateTime(2012, 06, 2);

    private readonly DateTime Sunday_2012_06_03 = new DateTime(2012, 06, 3);

    [Test]
    public void GetDaysTo__Returns_0_For_The_Same_Day()
    {
      Assert.AreEqual(0, Monday.GetDaysTo(Monday));
    }

    [Test]
    public void GetDaysTo__Returns_Correct_Values_For_All_Days_Of_A_Week()
    {
      Assert.AreEqual(0, Monday.GetDaysTo(Monday));
      Assert.AreEqual(1, Monday.GetDaysTo(Tuesday));
      Assert.AreEqual(2, Monday.GetDaysTo(Wednesday));
      Assert.AreEqual(3, Monday.GetDaysTo(Thursday));
      Assert.AreEqual(4, Monday.GetDaysTo(Friday));
      Assert.AreEqual(5, Monday.GetDaysTo(Saturday));
      Assert.AreEqual(6, Monday.GetDaysTo(Sunday));

      Assert.AreEqual(6, Tuesday.GetDaysTo(Monday));
      Assert.AreEqual(0, Tuesday.GetDaysTo(Tuesday));
      Assert.AreEqual(1, Tuesday.GetDaysTo(Wednesday));
      Assert.AreEqual(2, Tuesday.GetDaysTo(Thursday));
      Assert.AreEqual(3, Tuesday.GetDaysTo(Friday));
      Assert.AreEqual(4, Tuesday.GetDaysTo(Saturday));
      Assert.AreEqual(5, Tuesday.GetDaysTo(Sunday));

      Assert.AreEqual(5, Wednesday.GetDaysTo(Monday));
      Assert.AreEqual(6, Wednesday.GetDaysTo(Tuesday));
      Assert.AreEqual(0, Wednesday.GetDaysTo(Wednesday));
      Assert.AreEqual(1, Wednesday.GetDaysTo(Thursday));
      Assert.AreEqual(2, Wednesday.GetDaysTo(Friday));
      Assert.AreEqual(3, Wednesday.GetDaysTo(Saturday));
      Assert.AreEqual(4, Wednesday.GetDaysTo(Sunday));

      Assert.AreEqual(4, Thursday.GetDaysTo(Monday));
      Assert.AreEqual(5, Thursday.GetDaysTo(Tuesday));
      Assert.AreEqual(6, Thursday.GetDaysTo(Wednesday));
      Assert.AreEqual(0, Thursday.GetDaysTo(Thursday));
      Assert.AreEqual(1, Thursday.GetDaysTo(Friday));
      Assert.AreEqual(2, Thursday.GetDaysTo(Saturday));
      Assert.AreEqual(3, Thursday.GetDaysTo(Sunday));

      Assert.AreEqual(3, Friday.GetDaysTo(Monday));
      Assert.AreEqual(4, Friday.GetDaysTo(Tuesday));
      Assert.AreEqual(5, Friday.GetDaysTo(Wednesday));
      Assert.AreEqual(6, Friday.GetDaysTo(Thursday));
      Assert.AreEqual(0, Friday.GetDaysTo(Friday));
      Assert.AreEqual(1, Friday.GetDaysTo(Saturday));
      Assert.AreEqual(2, Friday.GetDaysTo(Sunday));

      Assert.AreEqual(2, Saturday.GetDaysTo(Monday));
      Assert.AreEqual(3, Saturday.GetDaysTo(Tuesday));
      Assert.AreEqual(4, Saturday.GetDaysTo(Wednesday));
      Assert.AreEqual(5, Saturday.GetDaysTo(Thursday));
      Assert.AreEqual(6, Saturday.GetDaysTo(Friday));
      Assert.AreEqual(0, Saturday.GetDaysTo(Saturday));
      Assert.AreEqual(1, Saturday.GetDaysTo(Sunday));

      Assert.AreEqual(1, Sunday.GetDaysTo(Monday));
      Assert.AreEqual(2, Sunday.GetDaysTo(Tuesday));
      Assert.AreEqual(3, Sunday.GetDaysTo(Wednesday));
      Assert.AreEqual(4, Sunday.GetDaysTo(Thursday));
      Assert.AreEqual(5, Sunday.GetDaysTo(Friday));
      Assert.AreEqual(6, Sunday.GetDaysTo(Saturday));
      Assert.AreEqual(0, Sunday.GetDaysTo(Sunday));
    }

    [Test]
    public void Include__Is_False_For_Dates_After_Shift_With_DayChange()
    {
      const DayOfWeek day = Monday;
      var time = new DateTime(1, 1, 1, 22, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 22:00 - Di 06:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsFalse(sut.Includes(Tuesday_2012_05_22.AddHours(6)
                                                    .AddTicks(1)));

      //2nd week
      Assert.IsFalse(sut.Includes(Tuesday_2012_05_29.AddHours(6)
                                                    .AddTicks(1)));
    }

    [Test]
    public void Include__Is_False_For_Dates_After_Shift_Without_DayChange()
    {
      const DayOfWeek day = Tuesday;
      var time = new DateTime(1, 1, 1, 9, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 9:00-17:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsFalse(sut.Includes(Monday_2012_05_21.AddHours(17)
                                                   .AddTicks(1)));

      //2nd week
      Assert.IsFalse(sut.Includes(Monday_2012_05_28.AddHours(17)
                                                   .AddTicks(1)));
    }

    [Test]
    public void Include__Is_False_For_Dates_Before_Shift_With_DayChange()
    {
      const DayOfWeek day = Monday;
      var time = new DateTime(1, 1, 1, 22, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 22:00 - Di 06:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsFalse(sut.Includes(Monday_2012_05_21.AddHours(22)
                                                   .AddTicks(-1)));

      //2nd week
      Assert.IsFalse(sut.Includes(Monday_2012_05_28.AddHours(22)
                                                   .AddTicks(-1)));
    }

    [Test]
    public void Include__Is_False_For_Dates_Before_Shift_Without_DayChange()
    {
      const DayOfWeek day = Tuesday;
      var time = new DateTime(1, 1, 1, 9, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 9:00-17:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsFalse(sut.Includes(Monday_2012_05_21.AddHours(9)
                                                   .AddTicks(-1)));

      //2nd week
      Assert.IsFalse(sut.Includes(Monday_2012_05_28.AddHours(9)
                                                   .AddTicks(-1)));
    }

    [Test]
    public void Include__Is_False_If_Given_Day_Is_Not_Inside_Shift()
    {
      const DayOfWeek day = Saturday;
      var time = new DateTime(1, 1, 1, 22, 0, 0);
      var duration = TimeSpan.FromHours(8);
//Sa 22:00 - Di 06:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      Assert.IsFalse(sut.Includes(Monday));
      Assert.IsFalse(sut.Includes(Tuesday));
      Assert.IsFalse(sut.Includes(Wednesday));
      Assert.IsFalse(sut.Includes(Thursday));
      Assert.IsFalse(sut.Includes(Friday));
    }

    [Test]
    public void Include__Is_True_For_Dates_In_Shift_With_DayChange()
    {
      const DayOfWeek day = Monday;
      var time = new DateTime(1, 1, 1, 22, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 22:00 - Di 06:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsTrue(sut.Includes(Monday_2012_05_21.AddHours(22)));
      Assert.IsTrue(sut.Includes(Monday_2012_05_21.AddHours(22)
                                                  .AddTicks(1)));


      Assert.IsTrue(sut.Includes(Tuesday_2012_05_22.AddHours(6)
                                                   .AddTicks(-1)));
      Assert.IsTrue(sut.Includes(Tuesday_2012_05_22.AddHours(6)));

      //2nd week
      Assert.IsTrue(sut.Includes(Monday_2012_05_28.AddHours(22)));
      Assert.IsTrue(sut.Includes(Monday_2012_05_28.AddHours(22)
                                                  .AddTicks(1)));


      Assert.IsTrue(sut.Includes(Tuesday_2012_05_29.AddHours(6)
                                                   .AddTicks(-1)));
      Assert.IsTrue(sut.Includes(Tuesday_2012_05_29.AddHours(6)));
    }

    [Test]
    public void Include__Is_True_For_Dates_In_Shift_With_DayChange_On_Saturday_Which_Is_DayOfWeek_MaxValue()
    {
      const DayOfWeek day = Saturday;
      var time = new DateTime(1, 1, 1, 22, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 22:00 - Di 06:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsTrue(sut.Includes(Saturday_2012_05_26.AddHours(22)));
      Assert.IsTrue(sut.Includes(Saturday_2012_05_26.AddHours(22)
                                                    .AddTicks(1)));


      Assert.IsTrue(sut.Includes(Sunday_2012_05_27.AddHours(6)
                                                  .AddTicks(-1)));
      Assert.IsTrue(sut.Includes(Sunday_2012_05_27.AddHours(6)));

      //2nd week
      Assert.IsTrue(sut.Includes(Saturday_2012_06_02.AddHours(22)));
      Assert.IsTrue(sut.Includes(Saturday_2012_06_02.AddHours(22)
                                                    .AddTicks(1)));


      Assert.IsTrue(sut.Includes(Sunday_2012_06_03.AddHours(6)
                                                  .AddTicks(-1)));
      Assert.IsTrue(sut.Includes(Sunday_2012_06_03.AddHours(6)));
    }

    [Test]
    public void Include__Is_True_For_Dates_In_Shift_Without_DayChange()
    {
      const DayOfWeek day = Monday;
      var time = new DateTime(1, 1, 1, 9, 0, 0);
      var duration = TimeSpan.FromHours(8);
      //Mo 9:00 - 17:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      //1st week
      Assert.IsTrue(sut.Includes(Monday_2012_05_21.AddHours(9)));
      Assert.IsTrue(sut.Includes(Monday_2012_05_21.AddHours(9)
                                                  .AddTicks(1)));


      Assert.IsTrue(sut.Includes(Monday_2012_05_21.AddHours(16)
                                                  .AddTicks(-1)));
      Assert.IsTrue(sut.Includes(Monday_2012_05_21.AddHours(17)));

      //2nd week
      Assert.IsTrue(sut.Includes(Monday_2012_05_28.AddHours(9)));
      Assert.IsTrue(sut.Includes(Monday_2012_05_28.AddHours(9)
                                                  .AddTicks(1)));


      Assert.IsTrue(sut.Includes(Monday_2012_05_28.AddHours(16)
                                                  .AddTicks(-1)));
      Assert.IsTrue(sut.Includes(Monday_2012_05_28.AddHours(17)));
    }

    [Test]
    public void Include__Is_True_If_Given_Day_Is_Inside_Shift()
    {
      const DayOfWeek day = Saturday;
      var time = new DateTime(1, 1, 1, 22, 0, 0);
      var duration = TimeSpan.FromHours(8);
//Sa 22:00 - Di 06:00
      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = day,
                  StartTime = time
                };

      Assert.IsTrue(sut.Includes(Saturday));
      Assert.IsTrue(sut.Includes(Sunday)); // shift includes daybreak!
    }

    [Test]
    public void Includes__Gives_Correct_Result_If_Shift_Involves_The_First_Day_Of_A_Month()
    {
      var lastDayOfPreviousMonth = new DateTime(2013, 01, 31); // a thursday;
      var duration = TimeSpan.FromHours(8);

      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = Thursday
                };
      Assert.IsTrue(sut.Includes(lastDayOfPreviousMonth));

      sut = new ShiftConfiguration
            {
              Duration = duration,
              StartDay = Wednesday,
              StartTime = new DateTime(1, 1, 1, 22, 0, 0)
            };
      Assert.IsTrue(sut.Includes(lastDayOfPreviousMonth));
    }

    [Test]
    public void Includes__Gives_Correct_Result_If_Shift_Involves_The_First_Day_Of_A_Year()
    {
      var lastDayOfPreviousYear = new DateTime(2012, 12, 31); // a monday;

      var duration = TimeSpan.FromHours(8);

      var sut = new ShiftConfiguration
                {
                  Duration = duration,
                  StartDay = Monday,
                };
      Assert.IsTrue(sut.Includes(lastDayOfPreviousYear));

      sut = new ShiftConfiguration
            {
              Duration = duration,
              StartDay = Sunday,
              StartTime = new DateTime(1, 1, 1, 22, 0, 0)
            };
      Assert.IsTrue(sut.Includes(lastDayOfPreviousYear));
    }

    [Test]
    public void TheDayAfter_Returns_The_Right_Day_For_Every_Day_Of_The_Week()
    {
      Assert.AreEqual(Monday, Utils.TheDayAfter(Sunday));
      Assert.AreEqual(Tuesday, Utils.TheDayAfter(Monday));
      Assert.AreEqual(Wednesday, Utils.TheDayAfter(Tuesday));
      Assert.AreEqual(Thursday, Utils.TheDayAfter(Wednesday));
      Assert.AreEqual(Friday, Utils.TheDayAfter(Thursday));
      Assert.AreEqual(Saturday, Utils.TheDayAfter(Friday));
      Assert.AreEqual(Sunday, Utils.TheDayAfter(Saturday));
    }


    [Test]
    public void TheDayBefore_Returns_The_Right_Day_For_Every_Day_Of_The_Week()
    {
      Assert.AreEqual(Monday, Utils.TheDayBefore(Tuesday));
      Assert.AreEqual(Tuesday, Utils.TheDayBefore(Wednesday));
      Assert.AreEqual(Wednesday, Utils.TheDayBefore(Thursday));
      Assert.AreEqual(Thursday, Utils.TheDayBefore(Friday));
      Assert.AreEqual(Friday, Utils.TheDayBefore(Saturday));
      Assert.AreEqual(Saturday, Utils.TheDayBefore(Sunday));
      Assert.AreEqual(Sunday, Utils.TheDayBefore(Monday));
    }
  }
}

// ReSharper restore InconsistentNaming