using System.ComponentModel.Composition;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class ThroughputsReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new Throughputs();
      return newInstance;
    }
  }
}