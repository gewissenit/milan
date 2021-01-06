using System.ComponentModel;

namespace Milan.Simulation.Resources
{
  public interface IProductTypeSpecificResource : INotifyPropertyChanged
  {
    IResourceType ResourceType { get; set; }
    IProductType ProductType { get; set; }
    IResourcePool ResourcePool { get; set; }
    int Amount {get; set; }
  }
}