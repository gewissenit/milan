namespace Milan.VisualModeling.ViewModels
{
  public interface ISnapBehaviour
  {
    ICoordinate GetSuitableAnchor(IVisual visual, ICoordinate reference = null);
  }
}