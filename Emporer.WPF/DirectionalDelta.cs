namespace Emporer.WPF
{
  /// <summary>
  ///   A command parameter that provides a distance and a direction.
  /// </summary>
  public class DirectionalDelta
  {
    public DirectionalDelta(Direction direction, double distance = 1)
    {
      Direction = direction;
      Distance = distance;
    }

    public Direction Direction { get; private set; }
    public double Distance { get; private set; }

    public override string ToString()
    {
      return string.Format("{0} ({1})", Direction, Distance);
    }
  }
}