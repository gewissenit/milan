#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Milan.Simulation.Factories;
using Milan.Simulation.Reporting;

namespace Milan.Simulation
{
  [Export(typeof(ISimulationService))]
  internal sealed class SimulationService : ISimulationService
  {
    public const string ExperimentDataFolderName = "Experiments";
    public const string EcoFactoryFolderName = "EcoFactory";
    private readonly string _dataFolder;
    private readonly IExperimentFactory _experimentFactory;
    private readonly IEnumerable<IReportDataProviderFactory> _reportDataProviderFactories;
    private int _runNumber;

    [ImportingConstructor]
    public SimulationService([Import] IExperimentFactory experimentFactory,
                             [ImportMany] IEnumerable<IReportDataProviderFactory> reportDataProviderFactories)
    {
      _experimentFactory = experimentFactory;
      _reportDataProviderFactories = reportDataProviderFactories;
      //todo: handle this via settings
      _dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), EcoFactoryFolderName, ExperimentDataFolderName);
    }

    public event EventHandler<BatchEventArgs> BatchCreated;
    public event EventHandler<BatchEventArgs> BatchDeleted;
    public event EventHandler<AggregateException> BatchError;

    public void DeleteBatch(IBatch batch)
    {
      RaiseBatchDeleted(batch);
      if (!Directory.Exists(batch.DataFolder))
      {
        return;
      }
      Directory.Delete(batch.DataFolder, true);
    }

    public void Start(IBatch batch)
    {
      Task.Factory.StartNew(() => Run(batch))
          .ContinueWith(bl =>
                        {
                          BatchError?.Invoke(this, bl.Exception);
                        },
                        TaskContinuationOptions.OnlyOnFaulted);
    }

    public IBatch CreateBatch(IModel model,
                              int numberOfRuns,
                              IStatisticalObserverFactory[] statisticalObserverFactories,
                              TimeSpan settlingTime)
    {
      var folderName = SanitizeFileName($"{DateTime.Now:yyyy-MM-dd HH.mm.ss} {model.Name}");
      var batchDataFolder = Path.Combine(_dataFolder, folderName);
      //todo: extract to factory
      var batch = new Batch(Guid.NewGuid()
                                .ToString(),
                            model,
                            batchDataFolder);

      //TODO: using this random number generator and NOT saving the result somewhere makes it impossible to rerun a batch.
      var rand = new Random();
      var rands = new List<int>();
      for (var i = 0; i < numberOfRuns; i++)
      {
        int randNumber;
        do
        {
          randNumber = rand.Next();
        } while (rands.Contains(randNumber));
        rands.Add(randNumber);
        batch.Add(_experimentFactory.Create(model, randNumber, statisticalObserverFactories, settlingTime.ToSimulationTimeSpan()));
      }

      RaiseBatchCreated(batch);
      return batch;
    }

    private void Run(IBatch batch)
    {
      foreach (var experiment in batch)
      {
        var experimentFolder = Path.Combine(batch.DataFolder, experiment.Id.ToString());
        experiment.DataFolder = experimentFolder;
      }

      batch.Run();

      foreach (var observer in batch.SelectMany(exp => exp.Model.Observers)
                                    .Where(o => o.IsEnabled))
      {
        observer.Flush();
      }

      GenerateReportData(batch, _reportDataProviderFactories.Select(rdpf => rdpf.Create()));
    }

    private void GenerateReportData(IBatch batch, IEnumerable<IReportDataProvider> reportDataProviders)
    {
      var dataProviders = reportDataProviders.ToArray();
      foreach (var dataProvider in dataProviders)
      {
        dataProvider.Prepare(batch);
      }
      // export data to Excel file
      if (!Directory.Exists(batch.DataFolder))
      {
        Directory.CreateDirectory(batch.DataFolder);
      }
      var filePath = Path.Combine(batch.DataFolder, $"Results_{_runNumber++}.xlsx");
      ExcelExport.WriteDataSetsToExcel(new FileInfo(filePath), dataProviders);
      // removing data from memory
      batch.Clear();
    }

    private void RaiseBatchCreated(IBatch batch)
    {
      BatchCreated?.Invoke(this, new BatchEventArgs(batch));
    }

    private void RaiseBatchDeleted(IBatch batch)
    {
      BatchDeleted?.Invoke(this, new BatchEventArgs(batch));
    }

    private static string SanitizeFileName(string fileName)
    {
      return Path.GetInvalidFileNameChars()
                 .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
    }
  }
}