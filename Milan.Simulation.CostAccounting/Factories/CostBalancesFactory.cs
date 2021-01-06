using System.ComponentModel.Composition;
using Milan.Simulation.CostAccounting.ReportDataProviders;
using Milan.Simulation.Factories;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.CostAccounting.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class CostBalancesFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new CostBalances();
      return newInstance;
    }
  }
}