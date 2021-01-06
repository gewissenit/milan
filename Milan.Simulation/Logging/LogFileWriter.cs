#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Ionic.Zip;

namespace Milan.Simulation.Logging
{
  internal class LogFileWriter : ILogFileWriter
  {
    private const int DefaultBufferSize = 20000;
    private readonly ReaderWriterLockSlim _lock;
    private readonly Queue<string> _logBuffer;
    protected int _bufferSize = DefaultBufferSize;
    protected string _logFileExtension;
    protected string _logFileLocation;
    protected string _logFileName;
    protected string _logFileZipExtension;
    private Queue<string> _writeBuffer;

    public LogFileWriter()
    {
      _lock = new ReaderWriterLockSlim();
      _logBuffer = new Queue<string>();
      _writeBuffer = new Queue<string>(_bufferSize);
    }

    public void Append(string message)
    {
      _logBuffer.Enqueue(message);

      if (_logBuffer.Count <= _bufferSize)
      {
        return;
      }

      WriteBufferToFile();
    }

    /// <summary>
    ///   Configures the log writer.
    /// </summary>
    /// <param name="filename">The name (without extension) of the log file.</param>
    /// <param name="fileLocation">The folder where the log file will be written. [default: the systems "MyDocuments" folder]</param>
    /// <param name="extension">The file extension of the log file. [default: "log"]</param>
    /// <param name="zippedExtension">The extension of the zipped version of the log file. [default: "zip"]</param>
    /// <param name="bufferSize">The size of the write buffer.</param>
    public void Configure(string filename, string fileLocation, string extension = "log", string zippedExtension = "zip", int bufferSize = 20000)
    {
      if (string.IsNullOrWhiteSpace(filename))
      {
        throw new ArgumentException("Please provide a valid filename.", filename);
      }

      _logFileName = filename;

      _logFileLocation = string.IsNullOrEmpty(fileLocation)
                           ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                           : fileLocation;

      if (!Directory.Exists(_logFileLocation))
      {
        Directory.CreateDirectory(_logFileLocation);
      }

      _logFileExtension = extension;
      _logFileZipExtension = zippedExtension;
      _bufferSize = bufferSize > 1
                      ? bufferSize
                      : 1;

      _writeBuffer = new Queue<string>(_bufferSize);
    }

    public void Flush()
    {
      WriteBufferToFile();
      ZipLogFile();
    }

    protected void Flush(Queue<string> buffer)
    {
      _lock.EnterWriteLock();
      var logfilePath = Path.Combine(_logFileLocation, _logFileName);
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
      while (_writeBuffer.Count < _bufferSize &&
             _logBuffer.Count > 0)
      {
        _writeBuffer.Enqueue(_logBuffer.Dequeue());
      }

      Flush(_writeBuffer);
    }

    private void ZipLogFile()
    {
      var zipFileName = $"{_logFileName}.{_logFileZipExtension}";
      var zipPath = Path.Combine(_logFileLocation, zipFileName);
      var logfilePath = Path.Combine(_logFileLocation, _logFileName);
      var fileNameInZip = $"{_logFileName}.{_logFileExtension}";

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