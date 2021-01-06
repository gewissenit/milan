#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Milan.JsonStore
{
  [Export(typeof (IDomainMessageBus))]
  internal class DomainMessageBus : IDomainMessageBus
  {
    private readonly Subject<object> _entitiesAdded;
    private readonly Subject<object> _entitiesRemoved;
    private readonly Subject<PropertyChange> _propertyChanges;
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public DomainMessageBus([Import] IJsonStore store)
    {
      _store = store;

      _entitiesAdded = new Subject<object>();
      _entitiesRemoved = new Subject<object>();
      _propertyChanges = new Subject<PropertyChange>();
      Initialize();

      _store.ProjectChanged.Subscribe(_ => _store.Content.OfType<INotifyPropertyChanged>()
                                                 .ForEach(AttachChangeListeners));
    }

    public IObservable<PropertyChange> PropertyChanges
    {
      get { return _propertyChanges; }
    }

    public IObservable<object> EntityAdded
    {
      get { return _entitiesAdded; }
    }

    public IObservable<object> EntityRemoved
    {
      get { return _entitiesRemoved; }
    }

    private void Initialize()
    {
      var entityAdded = _store.ItemAdded.OfType<object>();

      var entityRemoved = _store.ItemRemoved.OfType<object>();
      entityAdded.Subscribe(_entitiesAdded.OnNext);
      entityAdded.Where(e => e is INotifyPropertyChanged)
                 .Cast<INotifyPropertyChanged>()
                 .Subscribe(AttachChangeListeners); // attach to future entities

      entityRemoved.Subscribe(_entitiesRemoved.OnNext);
      entityRemoved.Where(e => e is INotifyPropertyChanged)
                   .Cast<INotifyPropertyChanged>()
                   .Subscribe(DetachChangeListeners);
    }


    private void AttachChangeListeners(INotifyPropertyChanged entity)
    {
      entity.PropertyChanged += PropagatePropertyChanged;
    }

    private void DetachChangeListeners(INotifyPropertyChanged entity)
    {
      entity.PropertyChanged -= PropagatePropertyChanged;
    }

    private void PropagatePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      _propertyChanges.OnNext(new PropertyChange(sender, e.PropertyName));
    }
  }
}