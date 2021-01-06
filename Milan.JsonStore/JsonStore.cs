#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zip;
using Milan.JsonStore.Properties;
using Newtonsoft.Json;

namespace Milan.JsonStore
{
  [Export(typeof (IJsonStore))]
  internal class JsonStore : IJsonStore
  {
    private const string ProjectFileName = "project.json";
    private readonly Encoding _defaultEncoding = Encoding.UTF8;
    private readonly Subject<object> _itemAdded = new Subject<object>();
    private readonly Subject<object> _itemRemoved = new Subject<object>();
    private readonly Subject<Unit> _projectChanged = new Subject<Unit>();

    private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
                                                                  {
                                                                    ObjectCreationHandling = ObjectCreationHandling.Reuse,
                                                                    TypeNameHandling = TypeNameHandling.All,
                                                                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                                                                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                                                                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                                                                  };

    private ObservableCollection<object> _content = new ObservableCollection<object>();

    public JsonStore()
    {
      Settings.Default.DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var args = Environment.GetCommandLineArgs();
      if (args.Length == 2 &&
          File.Exists(args[1]))
      {
        Load(args[1]);
      }
      else if (File.Exists(Settings.Default.LastSaveFile))
      {
        Load();
      }
      else
      {
        New();
      }
    }

    public IEnumerable<object> Content
    {
      get { return _content; }
    }

    public IObservable<Unit> ProjectChanged
    {
      get { return _projectChanged; }
    }

    public IObservable<object> ItemAdded
    {
      get { return _itemAdded; }
    }

    public IObservable<object> ItemRemoved
    {
      get { return _itemRemoved; }
    }

    public void Add(object item)
    {
      _content.Add(item);
      _itemAdded.OnNext(item);
    }

    public void Remove(object item)
    {
      _content.Remove(item);
      _itemRemoved.OnNext(item);
    }

    public bool UnsavedChanges
    {
      get
      {
        return (string.IsNullOrEmpty(Settings.Default.LastSaveFile) && Content.Any()) ||
               SerializeToJsonString(Content) != GetJsonString(Settings.Default.LastSaveFile);
      }
    }

    public void Load()
    {
      var fileName = Settings.Default.LastSaveFile;
      Load(fileName);
    }

    public void Load(string fileName)
    {
      if (!File.Exists(fileName))
      {
        throw new InvalidOperationException(
          "Loading a file without specifying a valid filename is not supported atm. Use Load() method for loading the last saved file or New() method for creating a new one.");
      }

      var json = GetJsonString(fileName);
      _content = DeserializeFromJsonString(FixFile(json));
      RaiseProjectChanged();

      SaveLastFilePath(fileName);
    }

    public void New()
    {
      Settings.Default.LastSaveFile = string.Empty;
      _content = new ObservableCollection<object>();
      RaiseProjectChanged();
    }

    public void Save(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        throw new InvalidOperationException("Saving a file without setting a filename is not supported atm.");
      }

      SerializeJsonString(Content, fileName);

      SaveLastFilePath(fileName);
    }

    public void Save()
    {
      Save(Settings.Default.LastSaveFile);
    }

    private string GetJsonString(string fileName)
    {
      string json;
      using (var zip = ZipFile.Read(fileName,
                                    new ReadOptions
                                    {
                                      Encoding = _defaultEncoding
                                    }))
      {
        var projectFile = zip.Entries.Single(e => e.FileName == ProjectFileName);

        using (var reader = projectFile.OpenReader())
        {
          using (var streamReader = new StreamReader(reader))
          {
            json = streamReader.ReadToEnd();
          }
        }
      }
      return json;
    }

    private static void SaveLastFilePath(string fileName)
    {
      Settings.Default.LastUsedPath = Directory.GetParent(fileName)
                                               .FullName;
      Settings.Default.LastSaveFile = fileName;
      Settings.Default.Save();
    }

    private void RaiseProjectChanged()
    {
      if (ProjectChanged != null)
      {
        _projectChanged.OnNext(Unit.Default);
      }
    }

    private ObservableCollection<object> DeserializeFromJsonString(string json)
    {
      var project = JsonConvert.DeserializeObject<ObservableCollection<object>>(json, _serializerSettings);
      return project;
    }

    private string FixFile(string json)
    {
      //TODO: extract to own component
      json = Regex.Replace(json,
                           @"Milan\.Simulation\.Distributions\.(\w+)DistributionConfiguration, Milan\.Simulation",
                           @"Emporer.Math.Distribution.$1DistributionConfiguration, Emporer.Math.Distribution");
      json = Regex.Replace(json,
                           @"Milan\.Simulation\.Common\.(I?)ProductType(\w*), Milan\.Simulation\.Common",
                           @"Milan.Simulation.$1ProductType$2, Milan.Simulation");
      json = Regex.Replace(json,
                           @"Milan\.Simulation\.Common\.(I?)Connection, Milan\.Simulation\.Common",
                           @"Milan.Simulation.$1Connection, Milan.Simulation");
      json = Regex.Replace(json,
                           @"Milan\.Simulation\.Common\.Observers\.(I?)ProductTerminationCriteria, Milan\.Simulation\.Common",
                           @"Milan.Simulation.Observers.$1ProductTerminationCriteria, Milan.Simulation");
      json = Regex.Replace(json,
                           @"Milan\.Simulation\.Common\.UI\.ViewModels\.NullProductType, Milan\.Simulation\.Common\.UI",
                           @"Milan.Simulation.NullProductType, Milan.Simulation");
      json = Regex.Replace(json, @"Milan\.Simulation\.Common", @"EcoFactory.Components");
      json = Regex.Replace(json, @"Milan\.Simulation\.Discrete", @"Milan.Simulation");
      json = Regex.Replace(json, @"ShortDescription", @"Description");
      json = Regex.Replace(json, @"m_", @"_");
      json = Regex.Replace(json, @"Projects\.(I?)Category, Emporer\.Projects", @"Material.$1Category, Emporer.Material");
      json = Regex.Replace(json, @"""Id"": null,", @"");
      json = Regex.Replace(json, @"{\s*""\$id"": ""\d*"",\s*""\$type"": ""Milan\.JsonStore\.Project, Milan\.JsonStore"",\s*""Content"": ", "");
      json = Regex.Replace(json,
                           @",\s*""ExtendingProperties"": {\s*""\$id"": ""\d*"",\s*""\$type"": ""System.Collections.Generic.HashSet`1\[\[Milan.JsonStore.ExtendingPropertyValue, Milan.JsonStore\]\], System.Core"",\s*""\$values"": \[\]\s*}\s*}",
                           "");
      json = Regex.Replace(json, @"_Hour"": (\d+),\s*""_Minute"": (\d+)", @"StartTime"": ""0001-01-01T$1:$2:00""");
      json = Regex.Replace(json, @"StartTime"": ""0001-01-01T(\d):", @"StartTime"": ""0001-01-01T0$1:");
      json = Regex.Replace(json, @"StartTime"": ""0001-01-01T(\d+):(\d):", @"StartTime"": ""0001-01-01T$1:0$2:");
      json = Regex.Replace(json, @"_StartDay", @"StartDay");
      json = Regex.Replace(json, @"_Duration", @"Duration");
      return json;
    }

    private void SerializeJsonString(IEnumerable<object> project, string fileName)
    {
      var jsonString = SerializeToJsonString(project);

      using (var zip = new ZipFile())
      {
        zip.AddEntry(ProjectFileName, jsonString, _defaultEncoding);
        zip.Save(fileName);
      }
    }

    private string SerializeToJsonString(IEnumerable<object> project)
    {
      var persistString = JsonConvert.SerializeObject(project, Formatting.Indented, _serializerSettings);
      return persistString;
    }
  }
}