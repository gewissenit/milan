#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Statistics;

namespace Milan.Simulation.Observers
{
  public abstract class ProcessObserver<TEntity> : EntityTypeObserver<TEntity>
    where TEntity : IEntity
  {
    private readonly IList<Position> _finishedProcesses = new List<Position>();
    public IEnumerable<Position> FinishedPositions => _finishedProcesses;
    
    public IEnumerable<UnfinishedProcess> UnfinishedProcesses { get; private set; } = new UnfinishedProcess[0];

    public override string ToString()
    {
      return $"Observer: processes (for {typeof(TEntity).Name})";
    }

    public override void Reset()
    {
      _finishedProcesses.Clear();
      UnfinishedProcesses = new UnfinishedProcess[0];
      base.Reset();
    }

    protected override void OnEntityTypeEventOccurred(ISimulationEvent e)
    {
      var relatedEvent = e as IRelatedEvent;
      if (relatedEvent == null ||
          !(e.Sender is TEntity))
      {
        return;
      }

      var productRelated = e as IProductsRelatedEvent;
      if (productRelated != null)
      {
        foreach (var product in productRelated.Products)
        {
          CreateStatisticPosition(relatedEvent, product);
        }
      }
      else
      {
        CreateStatisticPosition(relatedEvent);
      }
    }


    protected override void OnSimulationEnd(ISimulationEvent endEvent)
    {
      /* We look for unfinished processes by filtering events by:
       *  - related to the observed entity type
       *  - are end events (IRelatedEvent)
       *  - whose related (start) event is not scheduled (if they are still scheduled they would start after sim end) */

      var relatedEvents = Scheduler.ScheduledItems.Where(s => s.Sender is TEntity)
                                         .OfType<IRelatedEvent>()
                                         .Where(r => !Scheduler.ScheduledItems.Contains(r.RelatedEvent))
                                         .ToArray();

      if (relatedEvents.Any())
      {
        var tEnd = CurrentExperiment.CurrentTime;
        UnfinishedProcesses = relatedEvents.Select(up => new UnfinishedProcess(((IEntity) up.Sender).Name, GetProcessName(up), (up as IProductsRelatedEvent)?.Products.Select(pt => pt.ProductType.Name)
                                                                                                                                                                  .Distinct()
                                                                                                                                                                  .ToArray() ?? new string[0], up.ScheduledTime - tEnd, tEnd - up.RelatedEvent.ScheduledTime));
      }
      else
      {
        UnfinishedProcesses = new UnfinishedProcess[0];
      }
    }
    
    private static string GetProcessName(ISimulationEvent endEvent)
    {
      return endEvent.Name.Substring(0, endEvent.Name.IndexOf(' '));
    }
    
    private void CreateStatisticPosition(IRelatedEvent e, Product product = null)
    {
      var productId = product?.Id ?? -1;
      var productType = product?.ProductType;

      var sp = new Position
               {
                 Station = (IEntity) e.Sender,
                 Experiment = CurrentExperiment,
                 Process = GetProcessName(e),
                 StartDate = e.RelatedEvent.ScheduledTime.ToRealDate(Model.StartDate),
                 EndDate = e.ScheduledTime.ToRealDate(Model.StartDate),
                 Event = e.Id,
                 ProductType = productType,
                 Product = productId
               };
      _finishedProcesses.Add(sp);
      //var ptName = sp.ProductType == null
      //               ? ""
      //               : sp.ProductType.Name;

      //var message = $"{sp.Station.Name};{sp.Process};{ptName};{sp.StartDate};{sp.EndDate};{sp.Duration};{sp.Product};{sp.Experiment.Id},{sp.Event}";
      //_logger.Append(message);
    }
  }
}