#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Caliburn.Micro;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ResourcePoolEditViewModel : StationaryElementEditViewModelBase<IResourcePool>
  {
    public ResourcePoolEditViewModel(IResourcePool model, IEnumerable<Screen> sections)
      : base(model, sections)
    {
    }
  }
}