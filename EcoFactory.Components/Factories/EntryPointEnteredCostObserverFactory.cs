using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.CostAccounting;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (ICostObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class EntryPointEnteredCostObserverFactory : ICostObserverFactory
  {
    public string Name
    {
      get { return "Entered"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (EntryPoint);
    }

    public ICostObserver Create()
    {
      return new CostObserverExtensions.EntryPoint.Entered();
    }

    public ICostObserver Duplicate(ICostObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (CostObserverExtensions.EntryPoint.Entered);
    }

    public ISimulationObserver CreateSimulationObserver(ISimulationObserver simulationObserver, IExperiment experiment)
    {
      var clone = Clone(simulationObserver);
      clone.Id = simulationObserver.Id;
      clone.Configure(experiment);
      return clone;
    }

    public void Prepare(ISimulationObserver simulationObserver)
    {
      Milan.Simulation.Utils.PrepareEntityObserver((IEntityObserver) simulationObserver);
      if (simulationObserver is IProductRelated)
      {
        Milan.Simulation.Utils.PrepareProductRelatedObserver((IProductRelated)simulationObserver);
      }
    }

    private ICostObserver Clone(ISimulationObserver costObserver)
    {
      var master = (CostObserverExtensions.EntryPoint.Entered) costObserver;
      var clone = new CostObserverExtensions.EntryPoint.Entered();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.CostAccounting.Utils.CloneCostObserver(clone, master);
      Milan.Simulation.Utils.CloneProductRelatedObserver(clone, master);
      return clone;
    }
  }
}