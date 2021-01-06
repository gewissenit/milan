namespace Emporer.Material.Factories
{
  public interface IPropertyTypeFactory
  {
    IPropertyType Create();
    IPropertyType Duplicate(IPropertyType master);
  }
}