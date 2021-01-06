using Emporer.Material;
using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting.ReportDataProviders
{
  public class MaterialPosition : Position
  {
    public virtual IMaterial Material { get; set; }
    public virtual double TotalCosts { get; set; }
    public virtual double LossCosts { get; set; }
  }
}