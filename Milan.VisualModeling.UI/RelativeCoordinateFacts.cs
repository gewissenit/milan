using Milan.VisualModeling.ViewModels;
using NUnit.Framework;

namespace Milan.VisualModeling.Tests
{
  [TestFixture]
  public class RelativeCoordinateFacts
  {
    private static readonly object[][] _input =
    {
      new object[]
      {
        0, 0, 0, 0
      },
      new object[]
      {
        1, 1, 0, 0
      },
      new object[]
      {
        0, 0, 1, 1
      },
      new object[]
      {
        -1, -1, 0, 0
      },
      new object[]
      {
        0, 0, -1, -1
      },
      new object[]
      {
        6.234, 346.6, 0.567, 0
      }
    };

    [Test]
    [TestCaseSource("_input")]
    public void It_can_be_created_from_a_reference_coordinate_and_an_offset(double refX, double refY, double dX, double dY)
    {
      var reference = new Coordinate(refX, refY);

      var sut = new RelativeCoordinate(reference, dX, dY);

      Assert.AreEqual(refX + dX, sut.X);
      Assert.AreEqual(refY + dY, sut.Y);
      Assert.AreEqual(dX, sut.OffsetX);
      Assert.AreEqual(dY, sut.OffsetY);
    }

    [Test]
    public void It_moves_when_its_reference_moves()
    {
      const double dX = 10.3;
      const double dY = 2.4;
      var reference = new Coordinate(1, 1);

      var sut = new RelativeCoordinate(reference, dX, dY);

      reference.X += 0.7;
      reference.Y += 0.6;

      Assert.AreEqual(1 + 10.3 + 0.7, sut.X);
      Assert.AreEqual(1 + 2.4 + 0.6, sut.Y);
      Assert.AreEqual(dX, sut.OffsetX);
      Assert.AreEqual(dY, sut.OffsetY);
    }

    [Test]
    public void It_raises_PropertyChanged_for_X_when_its_XOffset_is_changed()
    {
      var raised = false;
      var sut = new RelativeCoordinate(new Coordinate());

      sut.PropertyChanged += (s, e) =>
                             {
                               if (e.PropertyName != "X")
                               {
                                 raised = true;
                               }
                             };
      sut.OffsetX = 1;
      Assert.True(raised);
    }

    [Test]
    public void It_raises_PropertyChanged_for_Y_when_its_YOffset_is_changed()
    {
      var raised = false;
      var sut = new RelativeCoordinate(new Coordinate());

      sut.PropertyChanged += (s, e) =>
                             {
                               if (e.PropertyName != "Y")
                               {
                                 raised = true;
                               }
                             };
      sut.OffsetY = 1;
      Assert.True(raised);
    }
  }
}