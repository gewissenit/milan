namespace Emporer.Material.Factories
{
  public interface IMaterialPropertyFactory
  {
    IMaterialProperty Create();
    IMaterialProperty Duplicate(IMaterialProperty master);
  }
}