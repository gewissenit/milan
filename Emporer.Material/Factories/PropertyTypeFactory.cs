using System.ComponentModel.Composition;

namespace Emporer.Material.Factories
{
  [Export(typeof (IPropertyTypeFactory))]
  internal class PropertyTypeFactory : IPropertyTypeFactory
  {
    public IPropertyType Create()
    {
      var newInstance = new PropertyType();
      return newInstance;
    }

    public IPropertyType Duplicate(IPropertyType master)
    {
      var clone = Create();
      clone.DataSourceId = master.DataSourceId;
      clone.EcoCat = master.EcoCat;
      clone.EcoSubCat = master.EcoSubCat;
      clone.Location = master.Location;
      clone.Name = master.Name;
      clone.Unit = master.Unit;
      return clone;
    }
  }
}