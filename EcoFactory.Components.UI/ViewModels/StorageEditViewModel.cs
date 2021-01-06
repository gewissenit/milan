using System.Collections.Generic;
using Caliburn.Micro;
using Milan.Simulation.UI.ViewModels;

namespace EcoFactory.Components.UI.ViewModels
{
  public sealed class StorageEditViewModel : StationaryElementEditViewModelBase<IStorage>
  {
    public StorageEditViewModel(IStorage model, IEnumerable<Screen> sections)
      : base(model, sections)
    {
    }
  }
}