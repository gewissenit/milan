#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Logging
{
  public interface IExperimentLogWriterProvider
  {
    ILogFileWriter GetLogger(IExperiment experiment);
  }
}