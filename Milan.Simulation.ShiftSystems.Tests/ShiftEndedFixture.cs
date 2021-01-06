#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

using Milan.Simulation.Events;
using Milan.Simulation.ShiftSystems.SimulationEvents;
using NUnit.Framework;
using Rhino.Mocks;

// ReSharper disable InconsistentNaming

namespace Milan.Simulation.ShiftSystems.Tests
{
  [TestFixture]
  public class ShiftEndedFixture
  {
    [Test]
    public void Creating_ShiftEnded_Without_Related_StartEvent_Throws_InvalidOperationException()
    {
      var entityMock = MockRepository.GenerateMock<IEntity>();
      var shiftMock = MockRepository.GenerateMock<ShiftConfiguration>();
      Assert.Throws<ArgumentNullException>(()=>new ShiftEnded(entityMock, shiftMock, null));
    }

    [Test]
    public void Creating_ShiftRelated_Event_Without_Related_Shift_Throws_ArgumentNullException()
    {
      var entityMock = MockRepository.GenerateMock<IEntity>();
      var startEventStub = MockRepository.GenerateStub<ISimulationEvent>();
      Assert.Throws<ArgumentNullException>(() => new ShiftEnded(entityMock, null, startEventStub));
    }
  }
}

// ReSharper restore InconsistentNaming