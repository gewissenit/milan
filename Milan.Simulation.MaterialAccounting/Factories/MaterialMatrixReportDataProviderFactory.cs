using System.ComponentModel.Composition;
using Milan.Simulation.Factories;
using Milan.Simulation.MaterialAccounting.ReportDataProviders;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.MaterialAccounting.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class MaterialMatrixReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new MaterialMatrix();
      return newInstance;
    }
  }
}