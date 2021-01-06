using System.ComponentModel;

namespace Milan.VisualModeling.ViewModels
{
  /// <summary>
  /// A point on a plane (2D space).
  /// </summary>
  public interface ICoordinate:INotifyPropertyChanged
  {
    double X { get; set; }
    double Y { get; set; }
  }
}