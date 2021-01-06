using System.ComponentModel.Composition;
using Milan.JsonStore;
using Milan.Simulation.Factories;
using Milan.Simulation.MaterialAccounting.ReportDataProviders;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.MaterialAccounting.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class MaterialBalancesFactory : IReportDataProviderFactory
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public MaterialBalancesFactory([Import] IJsonStore store)
    {
      _store = store;
    }

    public IReportDataProvider Create()
    {
      var newInstance = new MaterialBalances(_store);
      return newInstance;
    }
  }
}