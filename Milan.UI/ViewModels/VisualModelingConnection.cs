using Milan.VisualModeling.ViewModels;

namespace Milan.UI.ViewModels
{
  public class VisualModelingConnection : Edge
  {
    public VisualModelingConnection(INode source, INode destination)
      : base(source, destination, null, new SnapEdgeToCenterOfSides())
    {
    }
  }
}