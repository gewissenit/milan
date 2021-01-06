using EcoFactory.Components;

namespace Milan.Simulation.MaterialAccounting.Tests
{
  internal class SUT : ProductTypeProcessMaterialObserver<IWorkstation, StartEventStub, EndEventStub>
  {
    public SUT()
      : base("Processing")
    {
    }
  }
}