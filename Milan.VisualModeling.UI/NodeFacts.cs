using System.Linq;
using System.Windows;
using Emporer.WPF.ViewModels;
using Milan.VisualModeling.Extensions;
using Milan.VisualModeling.Persistence;
using Milan.VisualModeling.ViewModels;
using Moq;
using NUnit.Framework;

namespace Milan.VisualModeling.Tests
{
  [TestFixture]
  public class NodeFacts
  {
    [Test]
    public void It_depends_on_a_view_model_and_a_configuration_during_construction()
    {
      const double x = 1.1;
      const double y = 2.2;
      const double width = 3.3;
      const double heigth = 4.4;


      var givenModelMock = new Mock<IViewModel>();
      var givenConfig = new VisualConfiguration(givenModelMock.Object)
                        {
                          X = x,
                          Y = y,
                          Width = width,
                          Height = heigth
                        };


      var sut = new Node(givenModelMock.Object, givenConfig);

      Assert.AreEqual(givenModelMock.Object, sut.Content);
      Assert.NotNull(sut.ConnectionAnchorPoints);
      Assert.IsNotEmpty(sut.ConnectionAnchorPoints);
      Assert.AreEqual(x, sut.Location.X);
      Assert.AreEqual(y, sut.Location.Y);
      Assert.AreEqual(width, sut.Width);
      Assert.AreEqual(heigth, sut.Height);
    }

    [Test]
    public void Is_updates_its_anchor_points_during_construction()
    {
      const double x = 0;
      const double y = 0;
      const double width = 2;
      const double heigth = 3;


      var givenModelMock = new Mock<IViewModel>();
      var givenConfig = new VisualConfiguration(givenModelMock.Object)
      {
        X = x,
        Y = y,
        Width = width,
        Height = heigth
      };


      var sut = new Node(givenModelMock.Object, givenConfig);

      Assert.True(sut.ConnectionAnchorPoints.Any(a=>a.IsEqualTo(new Point(0, 1.5))));
      Assert.True(sut.ConnectionAnchorPoints.Any(a=>a.IsEqualTo(new Point(2, 1.5))));
      Assert.True(sut.ConnectionAnchorPoints.Any(a=>a.IsEqualTo(new Point(1, 0))));
      Assert.True(sut.ConnectionAnchorPoints.Any(a=>a.IsEqualTo(new Point(1, 3))));
    }

    [Test]
    public void Its_anchor_points_update_if_its_width_is_changed()
    {
      const double x = 0;
      const double y = 0;
      const double width = 2;
      const double heigth = 2;


      var givenModelMock = new Mock<IViewModel>();
      var givenConfig = new VisualConfiguration(givenModelMock.Object)
      {
        X = x,
        Y = y,
        Width = width,
        Height = heigth
      };


      var sut = new Node(givenModelMock.Object, givenConfig);

      Assert.True(sut.ConnectionAnchorPoints.Any(a => a.IsEqualTo(new Point(1, 1))));
      sut.Width = 4;
      Assert.True(sut.ConnectionAnchorPoints.Any(a => a.IsEqualTo(new Point(2, 1))));
    }

    [Test]
    public void Its_anchor_points_update_when_its_height_is_changed()
    {

    }

    [Test]
    public void It_anchor_points_update_when_it_moves()
    {
      const double x = 0;
      const double y = 0;
      const double width = 2;
      const double heigth = 2;


      var givenModelMock = new Mock<IViewModel>();
      var givenConfig = new VisualConfiguration(givenModelMock.Object)
      {
        X = x,
        Y = y,
        Width = width,
        Height = heigth
      };


      var sut = new Node(givenModelMock.Object, givenConfig);

      Assert.True(sut.ConnectionAnchorPoints.Any(a => a.IsEqualTo(new Point(1, 1))));
      sut.Location.Y =1;
      Assert.True(sut.ConnectionAnchorPoints.Any(a => a.IsEqualTo(new Point(1, 2))));
    }
  }
}