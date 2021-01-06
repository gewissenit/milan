using System.ComponentModel.Composition;

namespace Emporer.Material.Factories
{
  [Export(typeof (IContainedMaterialFactory))]
  internal class ContainedMaterialFactory : IContainedMaterialFactory
  {
    public IContainedMaterial Create()
    {
      return new ContainedMaterial();
    }

    public IContainedMaterial Duplicate(IContainedMaterial containedMaterial)
    {
      return new ContainedMaterial
             {
               Amount = containedMaterial.Amount,
               Material = containedMaterial.Material
             };
    }
  }
}