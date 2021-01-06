using Emporer.WPF.Factories;
using System.ComponentModel.Composition;
using Emporer.WPF.ViewModels;
using Milan.Simulation;
using System.Collections.Generic;
using EcoFactory.Components.UI.ViewModels;

namespace EcoFactory.Components.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class MultipleEntititesSelectionEditViewModelFactory : IEditViewModelFactory
  {
    public bool CanHandle(object model)
    {
      return model as IEnumerable<IEntity> != null;
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new MultipleEntitiesSelectionViewModel(model as IEnumerable<IEntity>);
    }
  }
}
