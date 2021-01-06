using System.Collections.Generic;

namespace Milan.Simulation.Reporting
{
  public class Pivot
  {
    public string Name { get; set; }
    public IEnumerable<string> RowFieldNames { get; set; }
    public IEnumerable<DataField> DataFields { get; set; }
    public int StartRow { get; set; }
    public bool NoGrandTotals { get; set; }
  }
}