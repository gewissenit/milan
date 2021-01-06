using System.ComponentModel.Composition;
using Milan.Simulation;
using Milan.Simulation.Observers;
using Milan.Simulation.Factories;
using Milan.Simulation.MaterialAccounting;
using Milan.Simulation.MaterialAccounting.Factories;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IMaterialObserverFactory))][Export(typeof(ISimulationObserverFactory))]
  internal class AssemblyStationProcessingMaterialObserverFactory : IMaterialObserverFactory
  {
    public string Name
    {
      get { return "Processing"; }
    }

    public bool CanHandle(IEntity entity)
    {
      return entity.GetType() == typeof (AssemblyStation);
    }

    public IMaterialObserver Create()
    {
      return new MaterialObserverExtensions.AssemblyStation.Processing();
    }

    public IMaterialObserver Duplicate(IMaterialObserver materialObserver)
    {
      return Clone(materialObserver);
    }

    public bool CanHandle(ISimulationObserver simulationObserver)
    {
      return simulationObserver.GetType() == typeof (MaterialObserverExtensions.AssemblyStation.Processing);
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
      var observer = (MaterialObserverExtensions.AssemblyStation.Processing) simulationObserver;
      Milan.Simulation.Utils.PrepareEntityObserver(observer);
      Milan.Simulation.Utils.PrepareProductRelatedObserver(observer);
    }

    private IMaterialObserver Clone(ISimulationObserver costObserver)
    {
      var master = (MaterialObserverExtensions.AssemblyStation.Processing) costObserver;
      var clone = new MaterialObserverExtensions.AssemblyStation.Processing();
      Milan.Simulation.Utils.CloneEntityObserver(clone, master);
      Milan.Simulation.MaterialAccounting.Utils.CloneMaterialObserver(clone, master);
      Milan.Simulation.Utils.CloneProductRelatedObserver(clone, master);
      Milan.Simulation.Utils.CloneTimeReferencedObserver(clone, master);
      return clone;
    }
  }
}