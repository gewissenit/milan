#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Statistics;

namespace Milan.Simulation.Observers
{
  /// <summary>
  ///   Aggregates all cost balances stored in a products <see cref="Product.ExperimentProperties" />.
  ///   The balances are aggregated when a product leaves the model.
  ///   All products that remain in the model at experiment end are aggregated at once.
  /// </summary>
  public class ProductLifeCycleLogWriter : SchedulerLogWriter
  {
    private const string Line = "----------------------------------------------";
    private const string _logFileName = "ProductLifeCycles";

    protected override string LogFileName
    {
      get { return _logFileName; }
    }

    public override void Flush()
    {
      Append(Line);
      Append($"Finished experiment @{DateTime.Now}");
      Append(Line);
      base.Flush();
    }

    protected override void AdditionalInitialization()
    {
      base.AdditionalInitialization();

      var experimentId = CurrentExperiment.Id.ToString();
      
      var modelName = Model.Name;
      var modelDescription = Model.Description;

      Append($"Model name is: {modelName}");
      Append($"Model description is: {modelDescription}");
      Append($"Experiment id is: {experimentId}");
      Append(Environment.NewLine);
      Append(Line);
      Append($"Started experiment at {DateTime.Now}");
      Append($"Simulated start date is {Model.StartDate}");
      Append(Environment.NewLine);
      Append("Stationary elements:");
      Append(Environment.NewLine);
      var stationaryElements = Model.Entities.OfType<IStationaryElement>();

      foreach (var stationaryElement in stationaryElements)
      {
        Append($"{stationaryElement.Name} [{stationaryElement.GetType() .Name}]");
      }

      Append(Line);
      Append(Environment.NewLine);
    }

    protected override void OnEventOccurred(ISimulationEvent e)
    {
      string sender;
      if (e.Sender is IEntity)
      {
        sender = (e.Sender as IEntity).Name;
      }
      else
      {
        sender = "SYSTEM";
      }

      var tpEnd = e.ScheduledTime.ToRealDate(Model.StartDate);

      var productsDestroyed = e as ProductsDestroyedEvent;
      if (productsDestroyed != null)
      {
        foreach (var product in productsDestroyed.Products)
        {
          Append($"{product.ProductType.Name} [{product.Id}] was destroyed at {sender}");
          WriteLifeCycle(product, tpEnd, sender);
        }
        return; // necessary because ProductsDestroyedEvent:ThroughputEndEvent
      }

      var throughputEnd = e as ThroughputEndEvent;
      if (throughputEnd == null)
      {
        return;
      }

      foreach (var product in throughputEnd.Products)
      {
        Append($"{product.ProductType.Name} [{product.Id}] left successfully at {sender}");
        WriteLifeCycle(product, tpEnd, sender);
      }
    }

    protected override void OnSimulationEnd(ISimulationEvent endEvent)
    {
      Append(Line);

      var timeOfSimulationEnd = CurrentExperiment.CurrentTime.ToRealDate(Model.StartDate);

      Append($"Experiment finished at {timeOfSimulationEnd} (simulated time)");
      Append(Environment.NewLine);
      Append("Products remaining in the system:");
      foreach (var se in Model.Entities.OfType<IStationaryElement>())
      {
        foreach (var remainingProduct in se.ResidingProducts)
        {
          Append($"{remainingProduct.ProductType.Name} [{remainingProduct.Id}] remained at {se.Name}");
          WriteLifeCycle(remainingProduct, timeOfSimulationEnd, se.Name);
        }
      }
    }

    private void WriteLifeCycle(Product product, DateTime lcEnd, string lastKnownPosition)
    {
      var lcStart = product.TimeStamp.ToRealDate(Model.StartDate);
      Append($"entry: {lcStart:yyyy-MM-dd HH:mm:ss.fff} @ {product.EntryPoint.Name}");
      var lcDuration = lcEnd - lcStart;

      Append("processes:");
      WriteProcesses(product);
      Append("path:");
      WriteLocationChanges(product);

      Append($"exit:  {lcEnd:yyyy-MM-dd HH:mm:ss.fff} ({(int) lcDuration.TotalHours}:{lcDuration.Minutes:00}:{lcDuration.Seconds:00}) @ {lastKnownPosition}");

      Append(string.Empty);
    }


    private void WriteLocationChanges(Product product)
    {
      var locationChanges = product.ExperimentProperties.OfType<LocationChangeInfo>()
                                   .Where(lc => lc.Type == LocationChange.Arrival);
      foreach (var locationChange in locationChanges)
      {
        var time = locationChange.Time.ToRealDate(Model.StartDate);

        Append($"       {time:yyyy-MM-dd HH:mm:ss.fff}  {locationChange.Type} @ {locationChange.Location}  --> {product.ProductType.Name} [{product.Id}]");
      }
    }

    private void WriteProcesses(Product product)
    {
      var processes = product.ExperimentProperties.OfType<ProcessStatistic>()
                             .ToArray();

      if (!processes.Any())
      {
        Append($"No processing occurred for {product.ProductType.Name} [{product.Id}]");
        return;
      }


      foreach (var processStatistic in processes)
      {
        var place = processStatistic.Entity;
        var process = processStatistic.Process;
        var duration = processStatistic.Durations.Mean;
        var value = processStatistic.Durations.ValuesOverTime.First(); // contains only one value
        var end = value.PointInTime.ToRealDate(Model.StartDate);
        var start = end - duration;

        Append($"       {start:yyyy-MM-dd HH:mm:ss.fff} - {end:yyyy-MM-dd HH:mm:ss.fff} ({(int) duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}) {process} @ {place}  --> {product.ProductType.Name} [{product.Id}]");
      }
    }
  }
}