namespace Emporer.Material.Factories
{
  public interface IContainedMaterialFactory
  {
    IContainedMaterial Create();
    IContainedMaterial Duplicate(IContainedMaterial containedMaterial);
  }
}