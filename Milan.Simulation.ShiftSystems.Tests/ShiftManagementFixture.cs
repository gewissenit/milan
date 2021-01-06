#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Milan.Simulation.ShiftSystems.SimulationEvents;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.ShiftSystems.Tests
{
  // ReSharper disable InconsistentNaming
  [TestFixture]
  public class ShiftManagementFixture : SimulationEntityFixture
  {
    [SetUp]
    public override void Setup()
    {
      base.Setup();
    }

    [TearDown]
    public override void TearDown()
    {
      base.TearDown();
    }

    private readonly DateTime _لأضحى2012 = new DateTime(2012, 10, 26); //Eid al-Adha

    private readonly DateTime _חנוכה2012 = new DateTime(2012, 12, 9); //Chanukka

    private readonly DateTime _Christmas2012 = new DateTime(2012, 12, 25);

    private readonly DateTime _होली2012 = new DateTime(2012, 03, 08); //Holi

    private readonly DateTime _Silvester2012 = new DateTime(2012, 12, 31);

    /// <summary>
    ///   Creates the shift based on a simple definition text.
    ///   The definition has to be like 'Mo 09:00 8:00' for a Shift on Monday from 9 o'clock for 8 hours.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <returns></returns>
    private ShiftConfiguration CreateShift(string definition)
    {
      var definitions = definition.Split(' ');
      var day = definitions[0];
      var startDef = definitions[1].Split(':');
      var startHour = byte.Parse(startDef[0]);
      var startMinute = byte.Parse(startDef[1]);
      var startTime = new DateTime(1, 1, 1, startHour, startMinute, 0);
      var durationDef = definitions[2].Split(':');
      var durationHours = byte.Parse(durationDef[0]);
      var durationMinutes = byte.Parse(durationDef[1]);
      var duration = new TimeSpan(0, durationHours, durationMinutes, 0);

      DayOfWeek dow;
      switch (day)
      {
        case "Mo":
          dow = DayOfWeek.Monday;
          break;
        case "Tu":
          dow = DayOfWeek.Tuesday;
          break;
        case "We":
          dow = DayOfWeek.Wednesday;
          break;
        case "Th":
          dow = DayOfWeek.Thursday;
          break;
        case "Fr":
          dow = DayOfWeek.Friday;
          break;
        case "Sa":
          dow = DayOfWeek.Saturday;
          break;
        case "Su":
          dow = DayOfWeek.Sunday;
          break;
        default:
          throw new InvalidOperationException();
      }

      var shiftMock = _mockRepository.DynamicMock<ShiftConfiguration>();

      shiftMock.StartDay = dow;
      shiftMock.StartTime = startTime;
      shiftMock.Duration = duration;


      return shiftMock;
    }

    private void SetExperimentStartDate(DateTime startDate)
    {
      _modelMock.Expect(m => m.StartDate)
                 .Return(startDate)
                 .Repeat.Any();
    }


    [Test]
    public void AddHoliday_Adds_NonExistentDates()
    {
      var sut = new ShiftManagement();

      sut.AddHoliday(_Christmas2012);
      sut.AddHoliday(_Silvester2012);
      sut.AddHoliday(_חנוכה2012);
      sut.AddHoliday(_لأضحى2012);
      sut.AddHoliday(_होली2012);

      Assert.IsTrue(sut.Holidays.Contains(_Christmas2012));
      Assert.IsTrue(sut.Holidays.Contains(_Silvester2012));
      Assert.IsTrue(sut.Holidays.Contains(_חנוכה2012));
      Assert.IsTrue(sut.Holidays.Contains(_لأضحى2012));
      Assert.IsTrue(sut.Holidays.Contains(_होली2012));

      Assert.AreEqual(5, sut.Holidays.Count());
    }

    [Test]
    public void AddHoliday_Does_Not_Add_Existing_Dates()
    {
      var sut = new ShiftManagement();

      sut.AddHoliday(_لأضحى2012);
      sut.AddHoliday(_होली2012);

      sut.AddHoliday(_لأضحى2012);
      sut.AddHoliday(_होली2012);

      Assert.AreEqual(2, sut.Holidays.Count());
    }

    [Test]
    public void Add_Shift()
    {
      var sut = new ShiftManagement();
      var mockShift = MockRepository.GenerateMock<ShiftConfiguration>();
      sut.AddShift(mockShift);
      Assert.Contains(mockShift, sut.Shifts.ToArray());
      Assert.AreEqual(1, sut.Shifts.Count());
    }

    [Test]
    public void Between_Two_Consecutive_Shifts_No_WorkingTimeEndEvent_Is_Raised()
    {
      var shiftMock1 = CreateShift("Mo 06:00 3:00");
      var shiftMock2 = CreateShift("Mo 9:00 3:00");

      var startDate = new DateTime(2012, 05, 21, 0, 0, 0);
      var sut = new ShiftManagement();
      sut.AddShift(shiftMock1);
      sut.AddShift(shiftMock2);

      CreateExperimentEnvironmentWithSpyScheduler(sut);
      SetExperimentStartDate(startDate);

      const double h = 3600000d;

      const double start1 = 6 * h; // 6h until 1st shift 1 start
      const double start2 = 9 * h; // 9h until 1st shift 2 start
      const double end2 = (9 + 3) * h;


      _mockRepository.ReplayAll();

      sut.Initialize();

      RaiseExpectedEvent("ShiftStarted", start1);
      RaiseExpectedEvent("WorkingTimeStarted", start1);
      RaiseExpectedEvent("ShiftStarted", start2);
      RaiseExpectedEvent("ShiftEnded", start2);
      RaiseExpectedEvent("ShiftEnded", end2);
      RaiseExpectedEvent("WorkingTimeEnded", end2);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Fail_On_Add_Null_Shift()
    {
      var sut = new ShiftManagement();
      Assert.Throws<ArgumentNullException>(() => sut.AddShift(null));
    }

    [Test]
    public void Fail_On_Add_Shift_Twice()
    {
      var sut = new ShiftManagement();
      var mockShift = MockRepository.GenerateMock<ShiftConfiguration>();
      sut.AddShift(mockShift);
      Assert.Throws<ArgumentException>(() => sut.AddShift(mockShift));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_Shift()
    {
      var sut = new ShiftManagement();
      var mockShift = MockRepository.GenerateMock<ShiftConfiguration>();
      Assert.Throws<ArgumentException>(() => sut.RemoveShift(mockShift));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_Shift()
    {
      var sut = new ShiftManagement();
      Assert.Throws<ArgumentNullException>(() => sut.RemoveShift(null));
    }

    [Test]
    public void Initialize__Schedules_Immediate_ShiftStart_If_Experiment_Starts_Inside_Shift()
    {
      var shiftMock = CreateShift("Mo 07:00 8:00");
      const double durationOfShift = 28800000d; //8 hours
      var startDate = new DateTime(2012, 05, 21, 8, 0, 0);

      var sut = new ShiftManagement();
      sut.AddShift(shiftMock);
      CreateExperimentEnvironmentWithSpyScheduler(sut);
      SetExperimentStartDate(startDate);


      _mockRepository.ReplayAll();
      sut.Initialize();

      RaiseExpectedEvent("ShiftStarted", 0);
      RaiseExpectedEvent("WorkingTimeStarted", 0);
      RaiseExpectedEvent("ShiftEnded", durationOfShift);

      _mockRepository.VerifyAll();
    }

    [Test]
    //tests also, if at least one shift is configured but is not scheduled at simulation start, a worktime start event should not be scheduled at simulation start.
    public void Initialize__Schedules_ShiftStart_If_Shift_Is_Configured()
    {
      var shiftMock = CreateShift("Mo 09:00 8:00");
      var startDate = new DateTime(2012, 05, 21, 8, 0, 0);

      var sut = new ShiftManagement();
      sut.AddShift(shiftMock);
      CreateExperimentEnvironmentWithSpyScheduler(sut);
      SetExperimentStartDate(startDate);

      const double start = 3600000d; //1 hour
      const double duration = 28800000d; //8 hours


      _mockRepository.ReplayAll();
      sut.Initialize();

      Assert.AreEqual(1, _schedulerSpy.Count()); // shift start
      RaiseExpectedEvent("ShiftStarted", start);
      Assert.AreEqual(2, _schedulerSpy.Count()); // working time start, shift end
      RaiseExpectedEvent("WorkingTimeStarted", start);
      RaiseExpectedEvent("ShiftEnded", start + duration);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Initialize__Schedules_WorktimeStart_If_No_Shifts_Are_Configured()
    {
      var sut = new ShiftManagement();
      CreateExperimentEnvironmentWithSpyScheduler(sut);
      _mockRepository.ReplayAll();

      sut.Initialize();

      RaiseExpectedEvent("WorkingTimeStarted", 0);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void IsInitializedAfterDefaultCtor()
    {
      var sut = new ShiftManagement();

      Assert.IsNotNull(sut.Name);
      Assert.IsEmpty(sut.Name);
      Assert.IsFalse(sut.Shifts.Any());
      Assert.IsFalse(sut.Holidays.Any());
    }

    [Test]
    public void On_ShiftStart_Shift_Dependend_Entitites_Get_Notified()
    {
      var shiftMock = CreateShift("Mo 09:00 8:00");
      var startDate = new DateTime(2012, 05, 21, 8, 0, 0);
      var workingTimeDependendEntityMock = _mockRepository.StrictMultiMock<IEntity>(typeof (IShiftPlanAware));
      var sut = new ShiftManagement();
      sut.AddShift(shiftMock);

      CreateExperimentEnvironmentWithSpyScheduler(sut,
                                                  new[]
                                                  {
                                                    workingTimeDependendEntityMock
                                                  });
      SetExperimentStartDate(startDate);

      workingTimeDependendEntityMock.Expect(m => (m as IShiftPlanAware).OnShiftStarted(Arg<ShiftConfiguration>.Is.Same(shiftMock)));
      workingTimeDependendEntityMock.Expect(m => (m as IShiftPlanAware).OnShiftEnded(Arg<ShiftConfiguration>.Is.Same(shiftMock)));

      _mockRepository.ReplayAll();

      sut.Initialize();

      RaiseExpectedEvent("ShiftStarted");
      RaiseExpectedEvent("WorkingTimeStarted");
      RaiseExpectedEvent("ShiftEnded");
      RaiseExpectedEvent("WorkingTimeEnded");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void On_Shift_End_Worktime_Ends_If_No_Other_Shift_Is_Active()
    {
      var shiftMock = CreateShift("Mo 09:00 8:00");
      var startDate = new DateTime(2012, 05, 21, 8, 0, 0);
      const double start = 3600000d; //1 hour
      const double duration = 28800000d; //8 hours


      var sut = new ShiftManagement();
      sut.AddShift(shiftMock);
      CreateExperimentEnvironmentWithSpyScheduler(sut);
      SetExperimentStartDate(startDate);

      _mockRepository.ReplayAll();
      sut.Initialize();

      RaiseExpectedEvent("ShiftStarted", start);
      RaiseExpectedEvent("WorkingTimeStarted", start);
      RaiseExpectedEvent("ShiftEnded", start + duration);
      RaiseExpectedEvent("WorkingTimeEnded", start + duration);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void On_WorkingTimeStart_WorkingTimeDependend_Entitites_Get_Notified()
    {
      var shiftMock = CreateShift("Mo 09:00 8:00");
      var startDate = new DateTime(2012, 05, 21, 8, 0, 0);
      var workingTimeDependendEntityMock = _mockRepository.StrictMultiMock<IEntity>(typeof (IWorkingTimeDependent));

      var sut = new ShiftManagement();
      sut.AddShift(shiftMock);

      CreateExperimentEnvironmentWithSpyScheduler(sut,
                                                  new[]
                                                  {
                                                    workingTimeDependendEntityMock
                                                  });
      SetExperimentStartDate(startDate);

      workingTimeDependendEntityMock.Expect(m => (m as IWorkingTimeDependent).OnWorkingTimeStarted());
      workingTimeDependendEntityMock.Expect(m => (m as IWorkingTimeDependent).OnWorkingTimeEnded());

      _mockRepository.ReplayAll();

      sut.Initialize();

      RaiseExpectedEvent("ShiftStarted");
      RaiseExpectedEvent("WorkingTimeStarted");
      RaiseExpectedEvent("ShiftEnded");
      RaiseExpectedEvent("WorkingTimeEnded");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void RemoveHoliday_Ignores_NonExisting_Dates()
    {
      var sut = new ShiftManagement();
      sut.AddHoliday(_لأضحى2012);
      sut.AddHoliday(_होली2012);

      sut.RemoveHoliday(_חנוכה2012);
      sut.RemoveHoliday(_Christmas2012);


      Assert.IsTrue(sut.Holidays.Contains(_لأضحى2012));
      Assert.IsTrue(sut.Holidays.Contains(_होली2012));
      Assert.AreEqual(2, sut.Holidays.Count());
    }

    [Test]
    public void RemoveHoliday_Removes_Existing_Dates()
    {
      var sut = new ShiftManagement();
      sut.AddHoliday(_لأضحى2012);
      sut.AddHoliday(_होली2012);

      sut.RemoveHoliday(_होली2012);

      Assert.IsTrue(sut.Holidays.Contains(_لأضحى2012));
      Assert.AreEqual(1, sut.Holidays.Count());
    }

    [Test]
    public void Remove_Shift()
    {
      var sut = new ShiftManagement();
      var mockShift = MockRepository.GenerateMock<ShiftConfiguration>();
      sut.AddShift(mockShift);
      sut.RemoveShift(mockShift);
      Assert.IsEmpty(sut.Shifts.ToArray());
    }

    [Test]
    public void Reset__Does_Not_Throw_Exceptions()
    {
      var sut = new ShiftManagement();
      var mockShift = MockRepository.GenerateMock<ShiftConfiguration>();
      sut.AddShift(mockShift);
      sut.AddHoliday(_لأضحى2012);

      sut.Reset();
    }

    [Test]
    public void Start_And_Stop_First_Shift_And_Then_Start_Second_Shift()
    {
      var shiftMockMo = CreateShift("Mo 09:00 8:00");
      var shiftMockDi = CreateShift("Tu 09:00 8:00");
      var startDate = new DateTime(2013, 01, 01, 0, 0, 0);
      var workingTimeDependendEntityMock = _mockRepository.StrictMultiMock<IEntity>(typeof (IWorkingTimeDependent));

      var sut = new ShiftManagement();
      sut.AddShift(shiftMockMo);
      sut.AddShift(shiftMockDi);

      CreateExperimentEnvironmentWithSpyScheduler(sut,
                                                  new[]
                                                  {
                                                    workingTimeDependendEntityMock
                                                  });
      SetExperimentStartDate(startDate);

      workingTimeDependendEntityMock.Expect(m => (m as IWorkingTimeDependent).OnWorkingTimeStarted())
                                    .Repeat.Any();
      workingTimeDependendEntityMock.Expect(m => (m as IWorkingTimeDependent).OnWorkingTimeEnded())
                                    .Repeat.Any();

      _discreteExperimentMock.Expect(e => e.CurrentTime)
                              .Return(0)
                              .Repeat.Times(4);
      _discreteExperimentMock.Expect(e => e.CurrentTime)
                              .Return(32400000)
                              .Repeat.Times(2);
      _discreteExperimentMock.Expect(e => e.CurrentTime)
                              .Return(61200000)
                              .Repeat.Times(3);
      _discreteExperimentMock.Expect(e => e.CurrentTime)
                              .Return(550800000)
                              .Repeat.Times(2);
      _discreteExperimentMock.Expect(e => e.CurrentTime)
                              .Return(579600000)
                              .Repeat.Times(3);

      _mockRepository.ReplayAll();

      sut.Initialize();
      //firs time
      RaiseExpectedEvent("ShiftStarted");
      RaiseExpectedEvent("WorkingTimeStarted");
      RaiseExpectedEvent("ShiftEnded");
      RaiseExpectedEvent("WorkingTimeEnded");
      //second time
      RaiseExpectedEvent("ShiftStarted");
      RaiseExpectedEvent("WorkingTimeStarted");
      RaiseExpectedEvent("ShiftEnded");
      RaiseExpectedEvent("WorkingTimeEnded");

      _mockRepository.VerifyAll();
      Assert.AreEqual(1,
                      _schedulerSpy.OfType<ShiftStarted>()
                                    .Count(e => e.Shift == shiftMockMo && e.ScheduledTime == 1130400000));
      Assert.AreEqual(1,
                      _schedulerSpy.OfType<ShiftStarted>()
                                    .Count(e => e.Shift == shiftMockDi && e.ScheduledTime == 637200000));
    }
  }
}

// ReSharper restore InconsistentNaming