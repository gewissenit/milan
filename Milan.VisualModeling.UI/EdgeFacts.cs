using System.ComponentModel;
using Milan.VisualModeling.ViewModels;
using Moq;
using NUnit.Framework;

namespace Milan.VisualModeling.Tests
{
  [TestFixture]
  public class EdgeFacts
  {
    [SetUp]
    public void Setup()
    {
      GivenSource = new Mock<INode>();
      GivenDestination = new Mock<INode>();
      GivenModel = new Mock<object>();
      GivenSnapBehavior = new Mock<ISnapBehaviour>();
    }

    private Edge _sut;


    public Mock<ISnapBehaviour> GivenSnapBehavior { get; set; }
    public Mock<object> GivenModel { get; set; }
    public Mock<INode> GivenDestination { get; set; }
    public Mock<INode> GivenSource { get; set; }
    public Coordinate GivenSourceLocation { get; set; }

    private void WhenSutIsCreated()
    {
      _sut = new Edge(GivenSource.Object, GivenDestination.Object, GivenModel.Object, GivenSnapBehavior.Object);
    }

    private void ThenTheSourceAnchorIs(double x, double y)
    {
      Assert.AreEqual(x, _sut.SourceAnchor.X);
      Assert.AreEqual(y, _sut.SourceAnchor.Y);
    }

    private void WhenTheSourceAnchorCoordinateMovesTo(int x, int y)
    {
      GivenSourceLocation.X = x;
      GivenSourceLocation.Y = y;
    }


    [Test]
    public void It_finds_matching_anchors_at_source_and_destination_on_creation()
    {
      GivenSource.At(0, 0)
                 .WithBestAnchorAtItsLocation();

      GivenDestination.At(10, 10)
                      .WithBestAnchorAtItsLocation();

      WhenSutIsCreated();

      Assert.AreEqual(_sut.SourceAnchor.X, GivenSource.Object.Location.X);
      Assert.AreEqual(_sut.SourceAnchor.Y, GivenSource.Object.Location.Y);
      Assert.AreEqual(_sut.DestinationAnchor.X, GivenDestination.Object.Location.X);
      Assert.AreEqual(_sut.DestinationAnchor.Y, GivenDestination.Object.Location.Y);
    }


    [Test]
    public void It_moves_its_source_anchor_if_the_referenced_coordinte_changes()
    {
      GivenSourceLocation = new Coordinate(0, 0);

      GivenSource.At(GivenSourceLocation)
                 .WithBestAnchorAtItsLocation();

      GivenDestination.At(10, 10)
                      .WithBestAnchorAtItsLocation();

      WhenSutIsCreated();

      WhenTheSourceAnchorCoordinateMovesTo(1, 1);
      ThenTheSourceAnchorIs(1, 1);
    }
  }

  public static class MockExtensions
  {
    public static Mock<INode> At(this Mock<INode> mock, double x, double y)
    {
      mock.Setup(m => m.Location)
          .Returns(new Coordinate(x, y));
      return mock;
    }

    public static Mock<INode> At(this Mock<INode> mock, ICoordinate coordinate)
    {
      mock.Setup(m => m.Location)
          .Returns(coordinate);
      return mock;
    }


    public static Mock<INode> MoveTo(this Mock<INode> mock, double x, double y)
    {
      mock.Setup(m => m.Location)
          .Returns(new Coordinate(x, y));

      mock.Raise(m => m.Location.PropertyChanged += null, new PropertyChangedEventArgs("X"));
      mock.Raise(m => m.Location.PropertyChanged += null, new PropertyChangedEventArgs("Y"));

      return mock;
    }

    public static Mock<INode> WithBestAnchorAtItsLocation(this Mock<INode> mock)
    {
      mock.Setup(m => m.ConnectionAnchorPoints)
          .Returns(new[]
                   {
                     new RelativeCoordinate(mock.Object.Location)
                   });
      return mock;
    }
  }
}