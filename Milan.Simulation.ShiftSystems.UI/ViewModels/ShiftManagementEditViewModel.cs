#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Caliburn.Micro;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.ShiftSystems.UI.ViewModels
{
  public sealed class ShiftManagementEditViewModel : StationaryElementEditViewModelBase<IShiftManagement>
  {
    public ShiftManagementEditViewModel(IShiftManagement model, IEnumerable<Screen> sections)
      : base(model, sections)
    {
    }
  }
}