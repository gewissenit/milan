namespace Milan.JsonStore
{
  public class PropertyChange
  {
    public PropertyChange(object entity, string propertyName)
    {
      Entity = entity;
      PropertyName = propertyName;
    }

    public string PropertyName { get; private set; }
    public object Entity { get; private set; }
  }
}