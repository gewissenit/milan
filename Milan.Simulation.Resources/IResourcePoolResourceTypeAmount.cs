using System.ComponentModel;

namespace Milan.Simulation.Resources
{
  public interface IResourcePoolResourceTypeAmount : INotifyPropertyChanged
  {
    IResourcePool ResourcePool { get; set; }
    IResourceType ResourceType { get; set; }
    int Amount { get; set; }
  }
}