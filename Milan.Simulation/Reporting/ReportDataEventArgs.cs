#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Reporting
{
  public class ReportDataEventArgs : BatchEventArgs
  {
    public ReportDataEventArgs(IBatch batch, IReportDataProvider dataProvider)
      : base(batch)
    {
      DataProvider = dataProvider;
    }


    public IReportDataProvider DataProvider { get; private set; }
  }
}