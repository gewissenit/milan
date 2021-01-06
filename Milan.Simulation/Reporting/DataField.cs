
namespace Milan.Simulation.Reporting
{
  public class DataField
  {
    public DataField()
    {
      Function = Functions.Sum;
    }

    public string SourceName { get; set; }
    public string Name { get; set; }
    public string Format { get; set; }

    public Functions Function { get; set; }

    public enum Functions
    {
      Average,
      Count,
      CountNums,
      Max,
      Min,
      Product,
      None,
      StdDev,
      StdDevP,
      Sum,
      Var,
      VarP,
    }
  }
}