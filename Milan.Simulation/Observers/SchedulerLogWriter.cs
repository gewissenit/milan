#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Ionic.Zip;

namespace Milan.Simulation.Observers
{
  public abstract class SchedulerLogWriter : SchedulerObserver
  {
    private const string LogFileExtension = "log";
    private const string LogFileZipExtension = "zip";
    private const int BufferSize = 20000;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private readonly Queue<string> _logBuffer = new Queue<string>();
    private readonly Queue<string> _writeBuffer = new Queue<string>(BufferSize);
    private string _logFileLocation;
    
    protected abstract string LogFileName { get; }

    public override void Flush()
    {
      base.Flush();
      WriteBufferToFile();
      ZipLogFile();
    }
    public override void Reset()
    {
      _logBuffer.Clear();
      _writeBuffer.Clear();
      base.Reset();
    }

    protected override void AdditionalInitialization()
    {
      _logFileLocation = CurrentExperiment.DataFolder;
    }

    protected void Append(string eventDescription)
    {
      _logBuffer.Enqueue(eventDescription);

      if (_logBuffer.Count <= BufferSize)
      {
        return;
      }

      WriteBufferToFile();
    }

    protected void Flush(Queue<string> buffer)
    {
      _lock.EnterWriteLock();
      var logfilePath = Path.Combine(_logFileLocation, LogFileName);
      using (var fs = new FileStream(logfilePath, FileMode.Append, FileAccess.Write, FileShare.None))
      {
        while (buffer.Count > 0)
        {
          // write a line in unicode
          var line = Encoding.Unicode.GetBytes(buffer.Dequeue() + "\r\n");
          fs.Write(line);
        }
      }
      _lock.ExitWriteLock();
    }

    private void WriteBufferToFile()
    {
      while (_writeBuffer.Count < BufferSize &&
             _logBuffer.Count > 0)
      {
        _writeBuffer.Enqueue(_logBuffer.Dequeue());
      }

      Flush(_writeBuffer);
    }

    private void ZipLogFile()
    {
      var zipFileName = $"{LogFileName}.{LogFileZipExtension}";
      var zipPath = Path.Combine(_logFileLocation, zipFileName);
      var logfilePath = Path.Combine(_logFileLocation, LogFileName);
      var fileNameInZip = $"{LogFileName}.{LogFileExtension}";

      using (var zipFile = new ZipFile())
      {
        using (var fs = new FileStream(logfilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          zipFile.AddEntry(fileNameInZip, fs);
          zipFile.Save(zipPath);
        }
      }

      File.Delete(logfilePath); // remove temporary log file
    }
  }
}