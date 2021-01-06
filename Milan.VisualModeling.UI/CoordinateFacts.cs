using Milan.VisualModeling.ViewModels;
using NUnit.Framework;

namespace Milan.VisualModeling.Tests
{
  [TestFixture]
  public class CoordinateFacts
  {
    [Test]
    public void It_notifies_when_it_moves_horizontally()
    {
      bool raised = false;
      var sut = new Coordinate();

      sut.PropertyChanged += (s, e) =>
                             {
                               raised = true;
                             };

      sut.X = 10;

      Assert.True(raised);
    }

    [Test]
    public void It_notifies_when_it_moves_vertically()
    {
      bool raised = false;
      var sut = new Coordinate();

      sut.PropertyChanged += (s, e) =>
                             {
                               raised = true;
                             };

      sut.Y = 10;

      Assert.True(raised);
    }
  }
}