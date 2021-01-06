using System.Collections.ObjectModel;
using System.Windows;
using Milan.VisualModeling.InputModes;
using Milan.VisualModeling.ViewModels;
using Moq;
using NUnit.Framework;
using GeWISSEN.TestUtils;

namespace Milan.VisualModeling.Tests
{
  [TestFixture]
  public class VisualEditorFacts
  {
    private VisualEditor _sut;
    private Window _window;
    private ObservableCollection<INode> GivenNodes { get; set; }
    private Size GivenSize { get; set; }

    [SetUp]
    public void Setup()
    {
      GivenNodes = new ObservableCollection<INode>();
    }

    private void WhenSutIsCreated()
    {
      _window = new Window();
      _sut = new VisualEditor();
      _sut.Nodes = GivenNodes;
      _window.Content = _sut;
    }

    [Test]
    public void It_has_no_dependencies()
    {
      SingleThreadApartmentRelatedTest.Run(() =>
      { WhenSutIsCreated(); });
    }

    [Test]
    public void It_moves_the_visible_area_to_the_left_when_instructed()
    {
      GivenSize = new Size(100, 100);
      GivenNodes.WithNode(200, 200, 20, 20);
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        WhenSutIsCreated();
      });
    }

    [Test]
    public void It_adjusts_its_canvas_size_when_instructed()
    {
      GivenNodes.WithNode(100, 100, 100, 100);
      SingleThreadApartmentRelatedTest.Run(() =>
      {
        WhenSutIsCreated();

        WhenCanvasSizeIsUpdatedExplicitely();

        ThenTheCanvasHasASizeOf(200, 200);
      });
    }

    private void ThenTheCanvasHasASizeOf(int width, int height)
    {
      Assert.AreEqual(width, _sut.Canvas.Width);
      Assert.AreEqual(height, _sut.Canvas.Height);
    }

    private void WhenCanvasSizeIsUpdatedExplicitely()
    {
      _sut.UpdateCanvasSize();
    }
  }
}