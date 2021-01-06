using System.Collections.ObjectModel;
using Emporer.WPF.ViewModels;
using Milan.VisualModeling.Persistence;
using Milan.VisualModeling.ViewModels;
using Moq;

namespace Milan.VisualModeling.Tests
{
  public static class VisualEditorContentHelpers
  {
    public static ObservableCollection<INode> WithNode(this ObservableCollection<INode> nodes, double x, double y, double width, double height)
    {
      nodes.Add(new Node(new Mock<IViewModel>().Object,
                         new VisualConfiguration(new object())
                         {
                           X = x,
                           Y = y,
                           Width = width,
                           Height = height
                         }));
      return nodes;
    }
  }
}