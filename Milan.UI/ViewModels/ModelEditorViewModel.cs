#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using EcoFactory.Components;
using Emporer.WPF;
using Milan.Simulation;
using Milan.Simulation.Resources;
using Milan.UI.Factories;
using Milan.UI.Views;
using Milan.VisualModeling;
using Milan.VisualModeling.Commands;
using Milan.VisualModeling.ViewModels;

namespace Milan.UI.ViewModels
{
  [Export(typeof (IModelEditorViewModel))]
  public sealed class ModelEditorViewModel : Screen, IModelEditorViewModel
  {
    private readonly IVisualModelingViewModelFactory _viewModelFactory;
    private AlignLeft _alignLeft;
    private IEnumerable<object> _selectedItems;
    private IModel _selectedModel;
    private ISelection _selection;
    private VisualEditor _visualEditor;


    [ImportingConstructor]
    public ModelEditorViewModel([Import] IVisualModelingViewModelFactory viewModelFactory)
    {
      _viewModelFactory = viewModelFactory;

      DisplayName = "Model Editor";
      Nodes = new ObservableCollection<INode>();
      Edges = new ObservableCollection<IEdge>();

      ViewAttached += InitializeEditorCommands;
    }

    //INFO: Bound (2way) to VisualEditor.SelectedItems in the view
    public IEnumerable<object> SelectedItems
    {
      get { return _selectedItems; }
      set
      {
        _selectedItems = value;
        NotifyOfPropertyChange(() => SelectedItems);

        if (_selectedItems == null)
        {
          return;
        }

        var selected = _selectedItems.ToArray();
        if (!selected.Any())
        { //nothing selected, select model
          if (_selectedModel != null)
          {
            _selection.Select((object) SelectedModel, this);
          }
          return;
        }
        // transform to models
        var models = selected.OfType<IVisual>()
                             .Select(v => v.Model)
                             .OfType<IEntity>()
                             .ToArray();

        _selection.Select(models, this);
      }
    }

    public ObservableCollection<INode> Nodes { get; private set; }

    public ObservableCollection<IEdge> Edges { get; private set; }

    public ICommand AlignLeft
    {
      get { return _alignLeft; }
    }

    public ICommand AlignHorizontallyCentered { get; private set; }

    public ICommand AlignRight { get; private set; }

    public ICommand AlignTop { get; private set; }

    public ICommand AlignVerticallyCentered { get; private set; }

    public ICommand DistributeHorizontally { get; private set; }

    public ICommand DistributeVertically { get; private set; }

    public ICommand AlignBottom { get; private set; }

    public IModel SelectedModel
    {
      get { return _selectedModel; }
      set
      {
        if (_selectedModel != null)
        {
          _selectedModel.ObservableEntities.CollectionChanged -= EntitiesChanged;
        }
        _selectedModel = value;

        if (_selectedModel != null)
        {
          _selectedModel.ObservableEntities.CollectionChanged += EntitiesChanged;
          UpdateToModel(_selectedModel);
        }

        NotifyOfPropertyChange(() => SelectedModel);

        if (_selection != null)
        {
          _selection.Select((object) _selectedModel, null);
        }
      }
    }
    
    public bool ModelIsEmpty
    {
      get { return !Nodes.Any(); }
    }

    public ISelection Selection
    {
      get { return _selection; }
      set
      {
        _selection = value;
        if (_selectedModel != null)
        {
          _selection.Select((object) _selectedModel, null);
        }
      }
    }
    
    private void InitializeEditorCommands(object sender, ViewAttachedEventArgs e)
    {
      ViewAttached -= InitializeEditorCommands;

      _visualEditor = ((ModelEditorView) e.View).VisualEditor;

      _alignLeft = new AlignLeft(_visualEditor);
      AlignHorizontallyCentered = new AlignHorizontallyCentered(_visualEditor);
      AlignRight = new AlignRight(_visualEditor);
      AlignTop = new AlignTop(_visualEditor);
      AlignVerticallyCentered = new AlignVerticallyCentered(_visualEditor);
      AlignBottom = new AlignBottom(_visualEditor);
      DistributeHorizontally = new DistributeHorizontally(_visualEditor);
      DistributeVertically = new DistributeVertically(_visualEditor);
    }

    private void UpdateToModel(IModel model)
    {
      Nodes.Clear();
      Edges.Clear();

      model.Entities.ForEach(AddEntity);
      NotifyOfPropertyChange(() => ModelIsEmpty);
      foreach (var edge in from source in model.Entities.OfType<INode<IConnection>>()
                           from destination in source.Connections.Select(con => con.Destination)
                           select _viewModelFactory.CreateEdge(GetNode(source), GetNode(destination)))
      {
        Edges.Add(edge);
      }

      foreach (var edge in from source in model.Entities.OfType<IWorkstationBase>()
                           from resourcePool in source.ProcessingResources.Concat(source.SetupResources)
                                                      .Select(rprta => rprta.ResourcePool)
                                                      .Distinct()
                           select _viewModelFactory.CreateEdge(GetNode(source), GetNode(resourcePool)))
      {
        Edges.Add(edge);
      }
    }

    private void EntitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        e.NewItems.OfType<IEntity>()
         .ForEach(AddEntity);
      }
      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        e.OldItems.OfType<IEntity>()
         .ForEach(RemoveEntity);
      }
      NotifyOfPropertyChange(() => ModelIsEmpty);
    }
    
    private void AddEntity(IEntity entity)
    {
      Nodes.Add(_viewModelFactory.CreateNode(entity));
      NotifyOfPropertyChange(() => Nodes);
      var node = entity as INode<IConnection>;
      if (node != null)
      {
        node.Added += AddConnection;
        node.Removed += RemoveConnection;
      }

      var ru = entity as IWorkstationBase;
      if (ru != null)
      {
        ru.ResourceAdded += AddResourceConnection;
        ru.ResourceRemoved += RemoveResourceConnection;
      }
    }

    private void RemoveConnection(INode<IConnection> source, IConnection connection)
    {
      var edge =
        Edges.SingleOrDefault(x => x.Source.Content.Model == source && x.Destination.Model == connection.Destination);
      if (edge == null)
      {
        return;
      }
      Edges.Remove(edge);
    }

    private void AddConnection(INode<IConnection> source, IConnection connection)
    {
      if (Edges.Any(x => x.Source.Content.Model == source && x.Destination.Model == connection.Destination))
      {
        return;
      }
      var edge = _viewModelFactory.CreateEdge(GetNode(source), GetNode(connection.Destination));
      Edges.Add(edge);
    }

    private void RemoveEntity(IEntity entity)
    {
      var nodeToRemove = Nodes.Single(x => x.Model == entity);
      Nodes.Remove(nodeToRemove);

      var node = entity as INode<IConnection>;
      if (node != null)
      {
        node.Added -= AddConnection;
        node.Removed -= RemoveConnection;
      }

      var ru = entity as IWorkstationBase;
      if (ru != null)
      {
        ru.ResourceAdded -= AddResourceConnection;
        ru.ResourceRemoved -= RemoveResourceConnection;
      }
    }

    private void RemoveResourceConnection(IWorkstationBase workstation, IResourcePool resourcePool)
    {
      if (workstation.ProcessingResources.Concat(workstation.SetupResources)
                     .Any(rprta => rprta.ResourcePool == resourcePool))
      {
        return;
      }

      var edge = Edges.SingleOrDefault(x => x.Source.Content.Model == workstation && x.Destination.Model == resourcePool);
      if (edge == null)
      {
        return;
      }
      Edges.Remove(edge);
    }

    private void AddResourceConnection(object sender, IResourcePool e)
    {
      var workstation = sender as IWorkstationBase;

      if (Edges.Any(x => x.Source.Content.Model == workstation && x.Destination.Model == e))
      {
        return;
      }

      var edge = _viewModelFactory.CreateEdge(GetNode(workstation), GetNode(e));
      Edges.Add(edge);
    }

    private INode GetNode(object content)
    {
      return Nodes.SingleOrDefault(x => x.Content.Model == content);
    }
  }
}