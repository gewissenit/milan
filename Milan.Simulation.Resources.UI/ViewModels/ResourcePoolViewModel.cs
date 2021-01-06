using System.Collections.Generic;
using Caliburn.Micro;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ResourcePoolViewModel : Screen
  {
    private readonly IResourcePool _resourcePool;

    public ResourcePoolViewModel(IResourcePool resourcePool)
    {
      _resourcePool = resourcePool;
    }

    public string Name
    {
      get { return _resourcePool.Name; }
      private set { _resourcePool.Name = value; }
    }

    public IEnumerable<IResourceTypeAmount> AvailableResourceTypeAmounts
    {
      get
      {        
        return _resourcePool.Resources;
      }
    }

    public override string DisplayName
    {
      get { return _resourcePool.Name; }
    }

    public IResourcePool Model
    {
      get { return _resourcePool; }
    }    

  }
}