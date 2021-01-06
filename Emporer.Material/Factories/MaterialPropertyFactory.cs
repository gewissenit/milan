using System.ComponentModel.Composition;

namespace Emporer.Material.Factories
{
  [Export(typeof (IMaterialPropertyFactory))]
  internal class MaterialPropertyFactory : IMaterialPropertyFactory
  {
    public IMaterialProperty Create()
    {
      var newInstance = new MaterialProperty();
      return newInstance;
    }

    public IMaterialProperty Duplicate(IMaterialProperty property)
    {
      return new MaterialProperty
             {
               Mean = property.Mean,
               PropertyType = property.PropertyType
             };
    }
  }
}