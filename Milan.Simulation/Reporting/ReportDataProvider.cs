#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Milan.Simulation.Reporting
{
  public abstract class ReportDataProvider: INotifyPropertyChanged, IReportDataProvider
  {
    protected IBatch _source;

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsPrepared { get; private set; }

    public IEnumerable<ReportDataSet> DataSets
    {
      get { return CreateDataSets(); }
    }
    
    public void Prepare(IBatch source)
    {
      _source = source;
      Prepare();
      IsPrepared = true;
    }
    
    protected abstract void Prepare();

    protected abstract IEnumerable<ReportDataSet> CreateDataSets();

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}