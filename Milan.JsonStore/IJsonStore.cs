#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reactive;

namespace Milan.JsonStore
{
  public interface IJsonStore
  {
    IEnumerable<object> Content { get; }
    IObservable<object> ItemAdded { get; }
    IObservable<object> ItemRemoved { get; }
    void Add(object item);
    void Remove(object item);
    void Load();
    void Load(string fileName);
    void New();
    void Save(string fileName);
    void Save();
    IObservable<Unit> ProjectChanged { get; }
    
    bool UnsavedChanges { get; }
  }
}