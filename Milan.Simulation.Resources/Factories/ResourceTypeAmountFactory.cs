using System.ComponentModel.Composition;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IResourceTypeAmountFactory))]
  internal class ResourceTypeAmountFactory : IResourceTypeAmountFactory
  {
    public IResourceTypeAmount Create()
    {
      var newInstance = new ResourceTypeAmount();
      return newInstance;
    }

    public IResourceTypeAmount Duplicate(IResourceTypeAmount master)
    {
      return new ResourceTypeAmount()
             {
               Amount = master.Amount,
               ResourceType = master.ResourceType
             };
    }
  }
}