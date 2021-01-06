using Newtonsoft.Json;

namespace Milan.VisualModeling.Persistence
{
  [JsonObject(MemberSerialization.OptOut)]
  public class VisualConfiguration
  {
    public VisualConfiguration(object model)
    {
      Model = model;
    }

    public object Model { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public override string ToString()
    {
      return string.Format("DTO Visual ({0}, at {1:0}|{2:0})", Model, X, Y);
    }
  }
}