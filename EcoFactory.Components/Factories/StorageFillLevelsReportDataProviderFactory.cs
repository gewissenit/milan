#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using EcoFactory.Components.ReportDataProviders;
using Milan.Simulation.Factories;
using Milan.Simulation.Reporting;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (IReportDataProviderFactory))]
  internal class StorageFillLevelsReportDataProviderFactory : IReportDataProviderFactory
  {
    public IReportDataProvider Create()
    {
      var newInstance = new StorageFillLevels();
      return newInstance;
    }
  }
}