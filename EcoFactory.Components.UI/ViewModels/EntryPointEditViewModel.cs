#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Caliburn.Micro;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  internal class EntryPointEditViewModel : StationaryElementEditViewModelBase<IEntryPoint>
  {
    public EntryPointEditViewModel(IEntryPoint model, IEnumerable<Screen> sections)
      : base(model, sections)
    {
    }
  }
}