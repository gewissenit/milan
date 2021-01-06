#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Logging
{
  public interface ILogFileWriter
  {
    void Append(string message);
    void Configure(string filename, string fileLocation, string extension = "log", string zippedExtension = "zip", int bufferSize = 20000);
    void Flush();
  }
}