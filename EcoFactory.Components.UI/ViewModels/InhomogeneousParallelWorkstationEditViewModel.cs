#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;
using Milan.Simulation.UI.ViewModels;
using System.Collections.Generic;

namespace EcoFactory.Components.UI.ViewModels
{
  internal class InhomogeneousParallelWorkstationEditViewModel : StationaryElementEditViewModelBase<IInhomogeneousParallelWorkstation>
  {
    public InhomogeneousParallelWorkstationEditViewModel(IInhomogeneousParallelWorkstation model, IEnumerable<Screen> sections)
      : base(model, sections)
    {
    }
  }
}