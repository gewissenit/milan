namespace Milan.Simulation.Reporting
{
  public class ReportDataSet
  {
    public ReportDataSet()
    {
      Pivots = new Pivot[0];
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] ColumnHeaders { get; set; }
    public Pivot[] Pivots { get; set; }
    public object[][] Data { get; set; }
  }
}