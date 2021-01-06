using Emporer.WPF.ViewModels;
using System.ComponentModel.Composition;
using Milan.Simulation.Resources.UI.ViewModels;
using Emporer.WPF.Factories;

namespace Milan.Simulation.Resources.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class InfluenceEditViewModelFactory : IEditViewModelFactory
  {
    public bool CanHandle(object model)
    {
      return model.GetType() == typeof(Influence);
    }

    public IEditViewModel CreateEditViewModel(object model)
    {
      return new InfluenceEditViewModel(model as IInfluence);
    }
  }
}
