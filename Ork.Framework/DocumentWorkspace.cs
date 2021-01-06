#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;

namespace Ork.Framework
{
  public abstract class DocumentWorkspace<TDocument> : Conductor<TDocument>.Collection.OneActive, IDocumentWorkspace
    where TDocument : class, INotifyPropertyChanged, IDeactivate, IHaveDisplayName
  {
    private DocumentWorkspaceState _state = DocumentWorkspaceState.Master;

    protected DocumentWorkspace()
    {
      Items.CollectionChanged += delegate
                                 {
                                   NotifyOfPropertyChange(() => Status);
                                 };
      DisplayName = Title;
    }

    public DocumentWorkspaceState State
    {
      get { return _state; }
      set
      {
        if (_state == value)
        {
          return;
        }

        _state = value;
        NotifyOfPropertyChange(() => State);
      }
    }

    public string Status
    {
      get
      {
        return Items.Count > 0
                 ? Items.Count.ToString()
                 : string.Empty;
      }
    }

    protected IConductor Conductor
    {
      get { return (IConductor) Parent; }
    }

    public abstract string Image { get; }
    public abstract int Index { get; }
    public virtual bool IsEnabled { get; protected set; }
    public abstract string Title { get; }

    void IDocumentWorkspace.Edit(object document)
    {
      Edit((TDocument) document);
    }

    public override void ActivateItem(TDocument item)
    {
      item.Deactivated += OnItemOnDeactivated;
      item.PropertyChanged += OnItemPropertyChanged;

      base.ActivateItem(item);
    }

    public void Edit(TDocument child)
    {
      Conductor.ActivateItem(this);
      State = DocumentWorkspaceState.Detail;
      DisplayName = child.DisplayName;
      ActivateItem(child);
    }

    public void Show()
    {
      var haveActive = Parent as IHaveActiveItem;
      if (haveActive != null &&
          haveActive.ActiveItem == this)
      {
        DisplayName = Title;
        State = DocumentWorkspaceState.Master;
      }
      else
      {
        Conductor.ActivateItem(this);
      }
    }

    private void OnItemOnDeactivated(object sender, DeactivationEventArgs e)
    {
      var doc = (TDocument) sender;
      if (e.WasClosed)
      {
        DisplayName = Title;
        State = DocumentWorkspaceState.Master;
        doc.Deactivated -= OnItemOnDeactivated;
        doc.PropertyChanged -= OnItemPropertyChanged;
      }
    }

    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "DisplayName")
      {
        DisplayName = ((TDocument) sender).DisplayName;
      }
    }

    public abstract void HandleKeyInput(Key key);
  }
}