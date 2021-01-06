#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Milan.Simulation.Events;

namespace Milan.Simulation.Observers
{
  public class EventLogWriter : SchedulerLogWriter
  {
    private const string _logFileName = "EventLog";

    protected override string LogFileName
    {
      get { return _logFileName; }
    }

    public override void Flush()
    {
      Append(Environment.NewLine);
      Append($"Finished experiment @{DateTime.Now}");
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
      Append(Environment.NewLine);
      Append($"Experiment id is: {experimentId}");
      Append($"Started experiment @{DateTime.Now}");
      Append(Environment.NewLine);
    }

    protected override void OnBeforeEventOccurred(ISimulationEvent e)
    {
      var senderName = GetSenderName(e);

      var message = $"[{e.ScheduledTime.ToRealTimeSpan():d\\.hh\\:mm\\:ss}] {senderName} : {e.Name} ({((SimulationEvent) e).Id})";
      Append(message);

      var productRelated = e as IProductsRelatedEvent;
      if (productRelated == null)
      {
        return;
      }

      Append("   Products:");

      foreach (var product in productRelated.Products)
      {
        Append($"      [{product.ProductType.Name}] (#{product.Id})  (currently at {product.CurrentLocation})");
      }
    }

    protected override void OnEventOccurred(ISimulationEvent e)
    {
      Append("------------------------");
    }

    protected override void OnEventsAdded(IEnumerable<ISimulationEvent> addedEvents)
    {
      foreach (var simulationEvent in addedEvents)
      {
        var entity = GetSenderName(simulationEvent);
        var eventType = simulationEvent.Name;
        Append($"   +{simulationEvent.ScheduledTime.ToRealTimeSpan():d\\.hh\\:mm\\:ss} {entity} : {eventType} ({((SimulationEvent) simulationEvent).Id})");
      }
    }

    protected override void OnEventsRemoved(IEnumerable<ISimulationEvent> removedEvents)
    {
      foreach (var simulationEvent in removedEvents)
      {
        var entity = GetSenderName(simulationEvent);
        var eventType = simulationEvent.Name;
        Append($"   -{simulationEvent.ScheduledTime.ToRealTimeSpan():d\\.hh\\:mm\\:ss} {entity} : {eventType} ({((SimulationEvent) simulationEvent).Id})");
      }
    }

    private static string GetSenderName(ISimulationEvent simulationEvent)
    {
      var eventName = "SYSTEM";
      if (simulationEvent.Sender != null)
      {
        var entity = simulationEvent.Sender as IEntity;
        eventName = entity != null
                      ? entity.Name
                      : simulationEvent.Sender.ToString();
      }
      return eventName;
    }
  }
}