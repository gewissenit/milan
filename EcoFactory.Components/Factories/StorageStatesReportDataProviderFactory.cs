using System.ComponentModel.Composition;
using EcoFactory.Components.ReportDataProviders;
using Milan.Simulation.Factories;
using Milan.Simulation.Reporting;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class StorageStatesReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new StorageStates();
      return newInstance;
    }
  }
}