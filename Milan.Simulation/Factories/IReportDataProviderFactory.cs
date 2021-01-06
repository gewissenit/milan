#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Reporting;

namespace Milan.Simulation.Factories
{
  public interface IReportDataProviderFactory
  {
    IReportDataProvider Create();
  }
}