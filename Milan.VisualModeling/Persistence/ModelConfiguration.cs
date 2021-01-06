using System.Collections.Generic;
using Newtonsoft.Json;

namespace Milan.VisualModeling.Persistence
{
  [JsonObject(MemberSerialization.OptOut)]
  public class ModelConfiguration
  {
    public ModelConfiguration()
    {
      Visuals = new List<VisualConfiguration>();
    }

    public ModelConfiguration(object model)
      : this()
    {
      Model = model;
    }

    public object Model { get; set; }
    public IList<VisualConfiguration> Visuals { get; private set; }

    public ModelConfiguration CloneFor(object model)
    {
      var modelConfiguration = new ModelConfiguration(model);
      return modelConfiguration;
    }

    public override string ToString()
    {
      return string.Format("DTO Visual Model ({0}, {1} Visuals)", Model, Visuals.Count);
    }
  }
}