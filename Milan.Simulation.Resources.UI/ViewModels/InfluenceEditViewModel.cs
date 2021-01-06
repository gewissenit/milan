using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class InfluenceEditViewModel : EntityEditViewModel
  {
    public InfluenceEditViewModel(IInfluence model)
      : base(model, "Influence")
    {
    }
  }
}