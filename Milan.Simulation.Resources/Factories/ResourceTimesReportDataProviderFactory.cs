using System.ComponentModel.Composition;
using Milan.Simulation.Factories;
using Milan.Simulation.Reporting;
using Milan.Simulation.Resources.ReportDataProviders;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class ResourceTimesReportDataProviderFactory: IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new ResourceTimes();
      return newInstance;
    }
  }
}