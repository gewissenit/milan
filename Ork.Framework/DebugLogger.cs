#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using Ork.Framework.Properties;

namespace Ork.Framework
{

  public class DebugLogger : ILog
  {
    private readonly Type _type;

    public DebugLogger(Type type)
    {
      _type = type;
    }

    public void Info(string format, params object[] args)
    {
      if (format.StartsWith("No bindable"))
        return;
      if (format.StartsWith("Action Convention Not Applied"))
        return;
      Debug.WriteLine("INFO: " + format, args);
    }

    public void Warn(string format, params object[] args)
    {
      Debug.WriteLine("WARN: " + format, args);
    }

    public void Error(Exception exception)
    {
      Debug.WriteLine("ERROR: {0}\n{1}", _type.Name, exception);
    }
  }
}