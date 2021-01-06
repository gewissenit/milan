using System.ComponentModel;
using Emporer.Math.Distribution;

namespace Milan.Simulation
{
  public interface INamedProcessConfiguration : INotifyPropertyChanged
  {
    string Name { get; }
    IDistributionConfiguration Duration { get; set; }
  }
}