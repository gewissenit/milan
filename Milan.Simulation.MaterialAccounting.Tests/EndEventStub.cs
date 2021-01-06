using System.Collections.Generic;
using Milan.Simulation.Events;

namespace Milan.Simulation.MaterialAccounting.Tests
{
  public class EndEventStub : ProductsRelatedEndEvent
  {
    private const string EventName = "Processing End";

    public EndEventStub(IEntity sender, IEnumerable<Product> relatedProducts, ISimulationEvent relatedStartEvent)
      : base(sender, EventName, relatedProducts, relatedStartEvent)
    {
    }
  }
}