#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.Events
{
  public class ProductsArrivedEvent : ProductsRelatedEvent
  {
    private const string EventName = "Products Arrived";


    public ProductsArrivedEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class BlockedStartEvent : SimulationEvent
  {
    private const string EventName = "Blocked Start";


    public BlockedStartEvent(IEntity sender)
      : base(sender, EventName)
    {
    }
  }

  public class BlockedEndEvent : RelatedEvent
  {
    private const string EventName = "Blocked End";


    public BlockedEndEvent(IEntity sender, ISimulationEvent relatedStartEvent)
      : base(sender, EventName, relatedStartEvent)
    {
    }
  }

  public class OffStartEvent : SimulationEvent
  {
    private const string EventName = "Off Start";

    public OffStartEvent(IEntity sender)
      : base(sender, EventName)
    {
    }
  }

  public class OffEndEvent : RelatedEvent
  {
    private const string EventName = "Off End";

    public OffEndEvent(IEntity sender, ISimulationEvent relatedStartEvent)
      : base(sender, EventName, relatedStartEvent)
    {
    }
  }

  public class IdleStartEvent : SimulationEvent
  {
    private const string EventName = "Idle Start";

    public IdleStartEvent(IEntity sender)
      : base(sender, EventName)
    {
    }
  }

  public class IdleEndEvent : RelatedEvent
  {
    private const string EventName = "Idle End";

    public IdleEndEvent(IEntity sender, ISimulationEvent relatedStartEvent)
      : base(sender, EventName, relatedStartEvent)
    {
    }
  }

  public class SetupCancelEvent : SimulationEvent
  {
    private const string EventName = "Setup Canceled";


    public SetupCancelEvent(IEntity sender)
      : base(sender, EventName)
    {
    }
  }

  public class SetupStartEvent : ProductsRelatedEvent
  {
    private const string EventName = "Setup Start";


    public SetupStartEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class SetupEndEvent : ProductsRelatedEndEvent
  {
    private const string EventName = "Setup End";


    public SetupEndEvent(IEntity sender, ISimulationEvent relatedStartEvent, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts, relatedStartEvent)
    {
    }
  }

  public class TransformationStartEvent : TransformationRelatedEvent
  {
    private const string EventName = "Transformation Start";

    public TransformationStartEvent(IEntity sender, IEnumerable<Product> inputProducts, ITransformationRule transformationRule)
      : base(sender, EventName, inputProducts, transformationRule)
    {
    }
  }

  public class TransformationEndEvent : TransformationRelatedEndEvent
  {
    private const string EventName = "Transformation End";

    public TransformationEndEvent(IEntity sender,
                                  IEnumerable<Product> outputProducts,
                                  ISimulationEvent relatedStartEvent,
                                  ITransformationRule transformationRule)
      : base(sender, EventName, outputProducts, relatedStartEvent, transformationRule)
    {
    }
  }

  public class ProcessingStartEvent : ProductsRelatedEvent
  {
    private const string EventName = "Processing Start";


    public ProcessingStartEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ProcessingResumeEvent : ProductsRelatedEvent
  {
    private const string EventName = "Processing Resumed";

    public ProcessingResumeEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ProcessingSuspendEvent : ProductsRelatedEvent
  {
    private const string EventName = "Processing Suspended";

    public ProcessingSuspendEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ProcessingEndEvent : ProductsRelatedEndEvent
  {
    private const string EventName = "Processing End";

    public ProcessingEndEvent(IEntity sender, IEnumerable<Product> relatedProducts, ISimulationEvent relatedStartEvent)
      : base(sender, EventName, relatedProducts, relatedStartEvent)
    {
    }
  }

  public class ParallelProcessingEndEvent : ProcessingEndEvent
  {
    private readonly double _duration;

    public ParallelProcessingEndEvent(IEntity sender, IEnumerable<Product> relatedProducts, ISimulationEvent relatedStartEvent, double duration)
      : base(sender, relatedProducts, relatedStartEvent)
    {
      _duration = duration;
    }

    public override double Duration
    {
      get { return _duration; }
    }
  }


  public class FailureEndEvent : RelatedEvent
  {
    private const string EventName = "Failure End";


    public FailureEndEvent(IEntity sender, ISimulationEvent relatedStartEvent)
      : base(sender, EventName, relatedStartEvent)
    {
    }
  }

  public class FailureStartEvent : SimulationEvent
  {
    private const string EventName = "Failure Start";


    public FailureStartEvent(IEntity sender)
      : base(sender, EventName)
    {
    }
  }
}