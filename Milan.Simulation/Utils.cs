using System.Collections.Generic;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation.Events;
using Milan.Simulation.Factories;
using Milan.Simulation.Observers;

namespace Milan.Simulation
{
  public static class Utils
  {
    public static TObserver GetObserver<TObserver>(this IExperiment experiment)
    {
      return experiment.Model.Observers.OfType<TObserver>()
                       .Single();
    }

    public static void CloneEntityObserver(IEntityObserver clone, IEntityObserver master)
    {
      clone.Entity = master.Entity;
    }

    public static void PrepareEntityObserver(IEntityObserver entityObserver)
    {
      entityObserver.Entity = entityObserver.Model.Entities.Single(e => e.Id == entityObserver.Entity.Id);
    }

    public static void Schedule(this ISimulationEvent simulationEvent, double timeDifference)
    {
      var experiment = ((IEntity) simulationEvent.Sender).CurrentExperiment;
      var scheduler = experiment.Scheduler;
      scheduler.Schedule(simulationEvent, timeDifference);
    }

    public static void ScheduleAfterCurrent(this ISimulationEvent simulationEvent)
    {
      var experiment = ((IEntity) simulationEvent.Sender).CurrentExperiment;
      var scheduler = experiment.Scheduler;
      scheduler.ScheduleAfterCurrent(simulationEvent);
    }

    public static void CloneTimeReferencedObserver(ITimeReferenced clone, ITimeReferenced master)
    {
      clone.TimeReference = master.TimeReference;
    }

    public static void PrepareStationaryElement(IStationaryElement stationaryElement)
    {
      foreach (var connection in stationaryElement.Connections)
      {
        var successorClone = stationaryElement.Model.Entities.OfType<IStationaryElement>()
                                              .Single(e => e.Id == connection.Destination.Id);
        foreach (var productType in connection.ProductTypes.ToArray())
        {
          var productTypeClone = stationaryElement.Model.Entities.OfType<IProductType>()
                                                  .Single(e => e.Id == productType.Id);
          connection.Remove(productType);
          connection.Add(productTypeClone);
        }
        connection.Destination = successorClone;
      }
    }

    public static IRealDistribution CreateAndSetupDistribution(IDistributionConfiguration distributionConfiguration, IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories, IEntity stationaryElement)
    {
      var dist = distributionFactories.Single(df => df.CanHandle(distributionConfiguration))
                                      .CreateAndConfigureDistribution(distributionConfiguration);
      dist.Seed = stationaryElement.CurrentExperiment.AcquireInitializationSeed(stationaryElement.Id.ToString());
      return dist;
    }

    public static void CloneStationaryElement(IStationaryElement master, IStationaryElement clone, IConnectionFactory connectionFactory)
    {
      foreach (var connection in master.Connections)
      {
        clone.Add(connectionFactory.Duplicate(connection));
      }
    }

    public static void CloneProductRelatedObserver(IProductRelated clone, IProductRelated master)
    {
      clone.IsProductTypeSpecific = master.IsProductTypeSpecific;
      clone.ProductType = master.ProductType;
      clone.QuantityReference = master.QuantityReference;
    }

    public static void PrepareProductRelatedObserver(IProductRelated productRelated)
    {
      if (!productRelated.IsProductTypeSpecific)
      {
        return;
      }
      //hack: cast to is ISimulationObserver has to be changed
      productRelated.ProductType = ((ISimulationObserver) productRelated).Model.Entities.OfType<IProductType>()
                                                                         .Single(e => e.Id == productRelated.ProductType.Id);
    }
  }
}