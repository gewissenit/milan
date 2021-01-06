using System.ComponentModel.Composition;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class EventsReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new Reporting.Events();
      return newInstance;
    }
  }
}