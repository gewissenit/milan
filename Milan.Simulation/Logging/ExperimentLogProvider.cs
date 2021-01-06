#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Milan.Simulation.Logging
{
  [Export(typeof (IExperimentLogWriterProvider))]
  internal class ExperimentLogProvider : IExperimentLogWriterProvider
  {
    private const string LogFolderName = "Logs";
    private readonly IDictionary<IExperiment, ILogFileWriter> _logFileWriters = new Dictionary<IExperiment, ILogFileWriter>();
    private string _localAppDataPath;

    [ImportingConstructor]
    public ExperimentLogProvider()
    {
      AssertLogFolderExistence();
    }

    public ILogFileWriter GetLogger(IExperiment experiment)
    {
      if (!_logFileWriters.ContainsKey(experiment))
      {
        _logFileWriters.Add(experiment, CreateLogger(experiment));
      }

      return _logFileWriters[experiment];
    }

    protected string GetLoggerName(IExperiment experiment)
    {
      return "ExperimentLog";
    }

    private void AssertLogFolderExistence()
    {
      _localAppDataPath = CreateLogFilePath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

      if (Directory.Exists(_localAppDataPath))
      {
        return;
      }

      try
      {
        Directory.CreateDirectory(_localAppDataPath);
      }
      catch (Exception)
      {
        _localAppDataPath = CreateLogFilePath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        Directory.CreateDirectory(_localAppDataPath);
      }
    }

    private string CreateLogFilePath(string baseFolder)
    {
      return Path.Combine(baseFolder, "EcoFactory", LogFolderName);
    }

    private ILogFileWriter CreateLogger(IExperiment experiment)
    {
      var logger = new LogFileWriter();

      logger.Configure(GetLoggerName(experiment), experiment.DataFolder);
      experiment.Finished += FinalizeLogFileWriter;
      return logger;
    }

    private void FinalizeLogFileWriter(object sender, EventArgs e)
    {
      var experiment = (IExperiment) sender;
      var logger = _logFileWriters[experiment];

      logger.Flush();

      _logFileWriters.Remove(experiment);
      //TODO: clean up the resource (IDisposable?)

      experiment.Finished -= FinalizeLogFileWriter; // unhook itself -> run only once
    }
  }
}