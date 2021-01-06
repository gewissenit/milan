using System.ComponentModel.Composition;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class TimesReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new Times();
      return newInstance;
    }
  }
}