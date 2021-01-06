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
  public class Bootstrapper : BootstrapperBase
  {
    private readonly Stopwatch _appRunTime = new Stopwatch();
    private IEnumerable<Assembly> _assemblies;
    private CompositionContainer _container;
    private Mutex _mutex; // has to stay here to prevent starting of second instance

    //static Bootstrapper()
    //{
    //  LogManager.GetLog = type => new Caliburn.Micro.Logging.DebugLogger(type);
    //}

    public Bootstrapper()
    {
      if (Settings.Default.AppFirstStartTime == new DateTime(2000, 1, 1))
      {
        Settings.Default.AppFirstStartTime = DateTime.Now;
        Settings.Default.Save();
      }
      _appRunTime.Start();
      Initialize();
    }

    public string ApplicationName
    {
      set
      {
        var batch = new CompositionBatch();
        batch.AddExportedValue("ApplicationName", value);
        _container.Compose(batch);
      }
    }

    public bool AllowMulitpleInstances { private get; set; }

    protected override void OnExit(object sender, EventArgs e)
    {
      SetAppRunTime();
      base.OnExit(sender, e);
    }

    protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      SetAppRunTime();
      base.OnUnhandledException(sender, e);
    }

    private void SetAppRunTime()
    {
      _appRunTime.Stop();
      Settings.Default.AppRunTime = Settings.Default.AppRunTime + TimeSpan.FromMilliseconds(_appRunTime.ElapsedMilliseconds);
      Settings.Default.Save();
    }

    protected override void BuildUp(object instance)
    {
      _container.SatisfyImportsOnce(instance);
    }

    protected override void Configure()
    {
      var assemblyCatalogs = _assemblies.Select(x => new AssemblyCatalog(x));
      var catalog = new AggregateCatalog(assemblyCatalogs);

      _container = new CompositionContainer(catalog);
      
      var batch = new CompositionBatch();
      batch.AddExportedValue<Func<IMessageBox>>(() => _container.GetExportedValue<IMessageBox>());
      batch.AddExportedValue<IWindowManager>(new MetroWindowManager());
      batch.AddExportedValue<IEventAggregator>(new EventAggregator());
      batch.AddExportedValue(_container);

      _container.Compose(batch);

      BindSpecialValueForKeyBindings();

      //INFO: see Caliburns internal log messages in the console
      //LogManager.GetLog = type => new DebugLogger(type);

      //INFO: break into view resolution
      //var original = ViewLocator.LocateForModel;
      //ViewLocator.LocateForModel = (o, dp, ob) =>
      //{
      //  return original(o, dp, ob);
      //};
    }

    protected override IEnumerable<object> GetAllInstances(Type serviceType)
    {
      return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
    }

    protected override object GetInstance(Type serviceType, string key)
    {
      var contract = string.IsNullOrEmpty(key)
                       ? AttributedModelServices.GetContractName(serviceType)
                       : key;
      var exports = _container.GetExportedValues<object>(contract);

      if (exports.Any())
      {
        return exports.First();
      }

      throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
    }

    protected override IEnumerable<Assembly> SelectAssemblies()
    {
      var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()
                                                  .Location);

      var directoryCatalog = new DirectoryCatalog(appPath);
      _assemblies = directoryCatalog.Parts.Select(part => ReflectionModelServices.GetPartType(part)
                                                                                 .Value.Assembly)
                                    .Distinct();
      return _assemblies;
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      if (AllowMulitpleInstances || !IsSecondInstance())
      {
        DisplayRootViewFor<IShell>();
      }
    }

    private bool IsSecondInstance()
    {
      var assembly = Assembly.GetEntryAssembly();
      var mutexName = String.Format(CultureInfo.InvariantCulture,
                                    "Local\\{{{0}}}{{{1}}}",
                                    assembly.GetType()
                                            .GUID,
                                    assembly.GetName()
                                            .Name);
      bool createdNew;
      _mutex = new Mutex(false, mutexName, out createdNew);
      if (!createdNew)
      {
        Application.Shutdown();
        return true;
      }
      return false;
    }

    private static void BindSpecialValueForKeyBindings()
    {
      //INFO: this enables an easy and reusable way of reacting to key bindings in CM screens
      //taken from http://stackoverflow.com/questions/16719496/caliburn-micro-enter-key-event
      MessageBinder.SpecialValues.Add("$pressedkey",
                                      (context) =>
                                      {
                                        var keyArgs = context.EventArgs as KeyEventArgs;

                                        if (keyArgs != null)
                                        {
                                          return keyArgs.Key;
                                        }

                                        return null;
                                      });
    }
  }
}