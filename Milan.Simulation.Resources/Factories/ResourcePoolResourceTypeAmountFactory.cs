using System.ComponentModel.Composition;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IResourcePoolResourceTypeAmountFactory))]
  internal class ResourcePoolResourceTypeAmountFactory : IResourcePoolResourceTypeAmountFactory
  {
    public IResourcePoolResourceTypeAmount Create()
    {
      return new ResourcePoolResourceTypeAmount();
    }

    public IResourcePoolResourceTypeAmount Duplicate(IResourcePoolResourceTypeAmount master)
    {
      return new ResourcePoolResourceTypeAmount()
             {
               ResourcePool = master.ResourcePool,
               Amount = master.Amount,
               ResourceType = master.ResourceType
             };
    }
  }
}