using System.ComponentModel.Composition;
using EcoFactory.Components.ReportDataProviders;
using Milan.Simulation.Factories;
using Milan.Simulation.Reporting;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class ProcessesReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new Processes();
      return newInstance;
    }
  }
}