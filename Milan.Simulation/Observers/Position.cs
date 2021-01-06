using System;
using Emporer.Unit;

namespace Milan.Simulation.Observers
{
  public class Position
  {
    public virtual long Event { get; set; }
    public virtual IEntity Station { get; set; }
    public virtual IProductType ProductType { get; set; }
    public virtual string Process { get; set; }
    public virtual string Category { get; set; }
    public virtual IUnit Currency { get; set; }
    public virtual long Product { get; set; }
    public virtual IExperiment Experiment { get; set; }
    public virtual double Total { get; set; }
    public virtual double Loss { get; set; }

    public virtual TimeSpan Duration
    {
      get
      {
        if (StartDate != default(DateTime))
        {
          return EndDate - StartDate;
        }
        else
        {
          return TimeSpan.Zero;
        }
      }
    }

    public virtual DateTime StartDate { get; set; }
    public virtual DateTime EndDate { get; set; }
    public virtual string ProductStatus { get; set; }
  }
}