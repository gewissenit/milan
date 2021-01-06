#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Milan.Simulation.Factories;

namespace Milan.Simulation.ShiftSystems.Factories
{
  [Export(typeof (IEntityFactory))]
  internal class ShiftManagementFactory : EntityFactory
  {
    [ImportingConstructor]
    public ShiftManagementFactory([ImportMany] IEnumerable<IAdditionalEntityDuplicationAction> additionalEntityDuplicationActions)
      : base("Shift Management", additionalEntityDuplicationActions)
    {
    }

    public override bool CanHandle(IEntity entity)
    {
      return entity is ShiftManagement;
    }

    private IEntity Clone(IEntity entity)
    {
      var master = (IShiftManagement) entity;
      var clone = new ShiftManagement();

      foreach (var dateTime in master.Holidays)
      {
        clone.AddHoliday(dateTime);
      }
      foreach (var shiftConfiguration in master.Shifts)
      {
        var scClone = new ShiftConfiguration
                      {
                        StartDay = shiftConfiguration.StartDay,
                        StartTime = shiftConfiguration.StartTime,
                        Duration = shiftConfiguration.Duration
                      };
        clone.AddShift(scClone);
      }
      return clone;
    }

    protected override IEntity Copy(IEntity entity)
    {
      return Clone(entity);
    }

    protected override IEntity CreateSimulationEntity(IEntity entity)
    {
      return Clone(entity);
    }

    protected override IEntity Create()
    {
      return new ShiftManagement();
    }
  }
}