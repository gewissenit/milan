#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Milan.VisualModeling.Shapes
{
  /// <summary>
  ///   Provides a base class for ArrowLine and ArrowPolyline.
  ///   This class is abstract.
  /// </summary>
  public abstract class ArrowLineBase : Shape
  {
    /// <summary>
    ///   Identifies the ArrowAngle dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrowAngleProperty = DependencyProperty.Register("ArrowAngle",
                                                                                               typeof (double),
                                                                                               typeof (ArrowLineBase),
                                                                                               new FrameworkPropertyMetadata(45.0,
                                                                                                                             FrameworkPropertyMetadataOptions
                                                                                                                               .AffectsMeasure));

    /// <summary>
    ///   Identifies the ArrowLength dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrowLengthProperty = DependencyProperty.Register("ArrowLength",
                                                                                                typeof (double),
                                                                                                typeof (ArrowLineBase),
                                                                                                new FrameworkPropertyMetadata(12.0,
                                                                                                                              FrameworkPropertyMetadataOptions
                                                                                                                                .AffectsMeasure));

    /// <summary>
    ///   Identifies the ArrowEnds dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrowEndsProperty = DependencyProperty.Register("ArrowEnds",
                                                                                              typeof (ArrowEnds),
                                                                                              typeof (ArrowLineBase),
                                                                                              new FrameworkPropertyMetadata(ArrowEnds.End,
                                                                                                                            FrameworkPropertyMetadataOptions
                                                                                                                              .AffectsMeasure));

    /// <summary>
    ///   Identifies the IsArrowClosed dependency property.
    /// </summary>
    public static readonly DependencyProperty IsArrowClosedProperty = DependencyProperty.Register("IsArrowClosed",
                                                                                                  typeof (bool),
                                                                                                  typeof (ArrowLineBase),
                                                                                                  new FrameworkPropertyMetadata(false,
                                                                                                                                FrameworkPropertyMetadataOptions
                                                                                                                                  .AffectsMeasure));

    private readonly PathFigure _pathfigHead1;
    private readonly PathFigure _pathfigHead2;
    private readonly PolyLineSegment _lineHeadStartFigure;
    private readonly PolyLineSegment _lineHeadEndFigure;
    protected readonly PathGeometry Geometry;
    protected readonly PolyLineSegment PolyLine;
    protected readonly PathFigure PathfigLine;

    /// <summary>
    ///   Initializes a new instance of ArrowLineBase.
    /// </summary>
    public ArrowLineBase()
    {
      Geometry = new PathGeometry();

      PathfigLine = new PathFigure();
      PolyLine = new PolyLineSegment();
      PathfigLine.Segments.Add(PolyLine);

      _pathfigHead1 = new PathFigure();
      _lineHeadStartFigure = new PolyLineSegment();
      _pathfigHead1.Segments.Add(_lineHeadStartFigure);

      _pathfigHead2 = new PathFigure();
      _lineHeadEndFigure = new PolyLineSegment();
      _pathfigHead2.Segments.Add(_lineHeadEndFigure);
    }

    /// <summary>
    ///   Gets or sets the angle between the two sides of the arrowhead.
    /// </summary>
    public double ArrowAngle
    {
      set { SetValue(ArrowAngleProperty, value); }
      get { return (double) GetValue(ArrowAngleProperty); }
    }

    /// <summary>
    ///   Gets or sets the length of the two sides of the arrowhead.
    /// </summary>
    public double ArrowLength
    {
      set { SetValue(ArrowLengthProperty, value); }
      get { return (double) GetValue(ArrowLengthProperty); }
    }

    /// <summary>
    ///   Gets or sets the property that determines which ends of the
    ///   line have arrows.
    /// </summary>
    public ArrowEnds ArrowEnds
    {
      set { SetValue(ArrowEndsProperty, value); }
      get { return (ArrowEnds) GetValue(ArrowEndsProperty); }
    }

    /// <summary>
    ///   Gets or sets the property that determines if the arrow head
    ///   is closed to resemble a triangle.
    /// </summary>
    public bool IsArrowClosed
    {
      set { SetValue(IsArrowClosedProperty, value); }
      get { return (bool) GetValue(IsArrowClosedProperty); }
    }

    /// <summary>
    ///   Gets a value that represents the Geometry of the ArrowLine.
    /// </summary>
    protected override Geometry DefiningGeometry
    {
      get
      {
        var count = PolyLine.Points.Count;

        if (count <= 0)
        {
          return Geometry;
        }
        // Draw the arrow at the start of the line.
        if ((ArrowEnds & ArrowEnds.Start) == ArrowEnds.Start)
        {
          var pt1 = PathfigLine.StartPoint;
          var pt2 = PolyLine.Points[0];
          Geometry.Figures.Add(CalculateArrow(_pathfigHead1, pt2, pt1));
        }

        // Draw the arrow at the end of the line.
        if ((ArrowEnds & ArrowEnds.End) == ArrowEnds.End)
        {
          var pt1 = count == 1
                      ? PathfigLine.StartPoint
                      : PolyLine.Points[count - 2];
          var pt2 = PolyLine.Points[count - 1];
          Geometry.Figures.Add(CalculateArrow(_pathfigHead2, pt1, pt2));
        }
        return Geometry;
      }
    }

    private PathFigure CalculateArrow(PathFigure pathfig, Point pt1, Point pt2)
    {
      var matx = new Matrix();
      var vect = pt1 - pt2;
      vect.Normalize();
      vect *= ArrowLength;

      var polyseg = pathfig.Segments[0] as PolyLineSegment;
      polyseg.Points.Clear();
      matx.Rotate(ArrowAngle / 2);
      pathfig.StartPoint = pt2 + vect * matx;
      polyseg.Points.Add(pt2);

      matx.Rotate(-ArrowAngle);
      polyseg.Points.Add(pt2 + vect * matx);
      pathfig.IsClosed = IsArrowClosed;

      return pathfig;
    }
  }
}