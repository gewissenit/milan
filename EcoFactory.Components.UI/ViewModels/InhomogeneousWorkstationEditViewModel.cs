#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.UI.ViewModels;
using Caliburn.Micro;

namespace EcoFactory.Components.UI.ViewModels
{
  public class InhomogeneousWorkstationEditViewModel : StationaryElementEditViewModelBase<IInhomogeneousWorkstation>
  {
    public InhomogeneousWorkstationEditViewModel(IInhomogeneousWorkstation model, IEnumerable<Screen> sections)
      : base(model, sections)
    {
    }
  }
}