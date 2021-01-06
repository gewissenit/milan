using System;
using Milan.VisualModeling.Extensions;
using Milan.VisualModeling.ViewModels;
using NUnit.Framework;

namespace Milan.VisualModeling.Tests
{
  [TestFixture]
  public class GeometryFacts
  {
    private static readonly object[] _adjacentCoordinates =
    {
      new object[] //identical
      {
        new Coordinate(0, 0), new[]
                              {
                                new Coordinate(0, 0)
                              },
        0
      },
      new object[] //duplicates
      {
        new Coordinate(0, 0), new[]
                              {
                                new Coordinate(0, 0), new Coordinate(0, 0)
                              },
        0
      },
      new object[] // +X
      {
        new Coordinate(0, 0), new[]
                              {
                                new Coordinate(1, 0), new Coordinate(2, 0)
                              },
        0
      },
      new object[] // -X
      {
        new Coordinate(0, 0), new[]
                              {
                                new Coordinate(-2, 0), new Coordinate(-1, 0)
                              },
        1
      },
      new object[] // +Y
      {
        new Coordinate(0, 0), new[]
                              {
                                new Coordinate(0, 1), new Coordinate(0, 2)
                              },
        0
      },
      new object[] // -Y
      {
        new Coordinate(0, 0), new[]
                              {
                                new Coordinate(0, -2), new Coordinate(0, -1)
                              },
        1
      },
    };

    [Test]
    [TestCaseSource("_adjacentCoordinates")]
    public void It_finds_the_nearest_of_a_set_of_coordinates_relative_to_a_reference_coordinate(Coordinate reference,
                                                                                                Coordinate[] candidates,
                                                                                                int nearestId)
    {
      Assert.AreEqual(candidates[nearestId], reference.GetNearest(candidates));
    }

    [Test]
    public void It_throws_ArgumentException_if_the_candidates_are_empty()
    {
      Assert.Throws<ArgumentException>(() => new Coordinate().GetNearest(new Coordinate[0]));
    }
  }
}