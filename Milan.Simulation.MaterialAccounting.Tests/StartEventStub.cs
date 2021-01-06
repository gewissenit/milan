using System.Collections.Generic;
using Milan.Simulation.Events;

namespace Milan.Simulation.MaterialAccounting.Tests
{
  public class StartEventStub : ProductsRelatedEvent
  {
    private const string EventName = "Processing Start";


    public StartEventStub(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }
}