namespace Milan.VisualModeling.ViewModels
{
  public class SnapEdgeToCenterOfSides : ISnapBehaviour
  {
    public ICoordinate GetSuitableAnchor(IVisual visual, ICoordinate reference=null)
    {
      if (reference==null)
      {
        return visual.Location;
      }

      return new Coordinate(visual.Bounds.TopLeft);
    }
  }
}