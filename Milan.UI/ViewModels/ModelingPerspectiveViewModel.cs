#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Emporer.WPF;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.Factories;
using Milan.Simulation.UI.ViewModels;
using Ork.Framework;

namespace Milan.UI.ViewModels
{
  [Export(typeof (IWorkspace))]
  public sealed class ModelingPerspectiveViewModel : DocumentBase, IWorkspace
  {
    private object _createEntityDialog;
    private IDictionary<string, KeyBinding> _keyBindings = new Dictionary<string, KeyBinding>();
    private IModel _selectedModel;

    [ImportingConstructor]
    public ModelingPerspectiveViewModel([Import] IModelEditorViewModel modelEditor,
                                        [Import] IPropertyEditorViewModel propertyEditorViewModel,
                                        [Import] ISelection modelingPerspectiveSelection,
                                        [Import] IJsonStore store,
                                        [Import] IModelFactory modelFactory,
                                        [ImportMany] IEnumerable<IEntityFactory> entityFactories,
                                        [Import] IDeleteManager deleteManager)
    {
      Index = 10;
      DisplayName = "Models";
      IsEnabled = true;

      ModelingPerspectiveSelection = modelingPerspectiveSelection;
      ModelEditor = modelEditor;
      PropertyEditor = propertyEditorViewModel;
      Store = store;
      ModelFactory = modelFactory;
      EntityFactories = entityFactories;
      DeleteManager = deleteManager;

      Models = new ObservableCollection<IModel>(Store.Content.OfType<IModel>());
      

      ModelEditor.DeactivateWith(this);

      PropertyEditor.Selection = ModelingPerspectiveSelection;
      ModelEditor.Selection = ModelingPerspectiveSelection;

      Store.ProjectChanged.Subscribe(_ => UpdateModelsFromStore());
      ModelingPerspectiveSelection.Subscribe<object>(this, SelectionChanged);

      SetupKeyBindings();

      SelectedModel = Models.FirstOrDefault();
    }


    public ObservableCollection<IModel> Models { get; set; }


    public IModelEditorViewModel ModelEditor { get; private set; }
    public IPropertyEditorViewModel PropertyEditor { get; private set; }
    public IJsonStore Store { get; set; }
    public IModelFactory ModelFactory { get; set; }
    public IEnumerable<IEntityFactory> EntityFactories { get; set; }
    public IDeleteManager DeleteManager { get; set; }
    public ISelection ModelingPerspectiveSelection { get; set; }

    public bool StartScreenShown
    {
      get
      {
        return !Models.Any();
      }
    }

    public IModel SelectedModel
    {
      get { return _selectedModel; }
      set
      {
        if (_selectedModel == value)
        {
          return;
        }
        _selectedModel = value;
        NotifyOfPropertyChange(() => SelectedModel);
        ModelEditor.SelectedModel = _selectedModel;
      }
    }

    public object CreateEntityDialog
    {
      get { return _createEntityDialog; }
      private set
      {
        _createEntityDialog = value;
        NotifyOfPropertyChange(() => CreateEntityDialog);
      }
    }

    public int Index { get; private set; }
    public bool IsEnabled { get; private set; }

    private void UpdateModelsFromStore()
    {
      Models.Clear();
      Store.Content.OfType<IModel>()
           .ForEach(Models.Add);
      SelectedModel = Models.FirstOrDefault();
      NotifyOfPropertyChange( () => StartScreenShown);
    }

    public bool CanAddEntity
    {
      get { return SelectedModel != null; }
    }

    public bool CanAddConnection
    {
      get { return ModelingPerspectiveSelection.Current is IStationaryElement; }
    }

    public bool CanDuplicateSelected
    {
      get {
        return ModelingPerspectiveSelection.Current is IModel
          || ModelingPerspectiveSelection.Current is IEntity
          || ModelingPerspectiveSelection.Current is IEnumerable<IEntity>; }
    }

    public bool CanRemoveSelected
    {
      get { return ModelingPerspectiveSelection.Current is IDomainEntity || ModelingPerspectiveSelection.Current is IEnumerable<IEntity>; }
    }

    public void AddEntity()
    {
      //todo: use factory
      //todo: this can be done once now
      CreateEntityDialog = new CreateEntityDialogViewModel(EntityFactories, SelectedModel);
    }

    public void AddModel()
    {
      var model = ModelFactory.Create();
      Models.Add(model);
      SelectedModel = model;
    }

    public void DuplicateSelected()
    {
      var model = ModelingPerspectiveSelection.Current as IModel;
      if (model != null)
      {
        var clone = ModelFactory.Duplicate(model);
        ModelingPerspectiveSelection.Select(clone, this);
        return;
      }

      var entities = ModelingPerspectiveSelection.Current as IEnumerable<IEntity>;
      if (entities == null)
      {
        return;
      }

      var duplicates = entities.Select(entity=>EntityFactories.Single(ef => ef.CanHandle(entity))
                                                              .Duplicate(entity, entity.Model))
                                                              .ToArray();

      ModelingPerspectiveSelection.Select(duplicates, this);
    }

    public void RemoveSelected()
    {
      if (ModelingPerspectiveSelection.Current is IModel)
      {
        Models.Remove((IModel) ModelingPerspectiveSelection.Current);
      SelectedModel = Models.FirstOrDefault();
        return;
      }

      var entities = ModelingPerspectiveSelection.Current as IEnumerable<IEntity>;
      if (entities == null)
      {
        return;
      }
      entities.ForEach(DeleteManager.Delete);
    }

    private void SelectionChanged(object obj)
    {
      NotifyOfPropertyChange(() => StartScreenShown);
      NotifyOfPropertyChange(() => CanAddEntity);
      NotifyOfPropertyChange(() => CanAddConnection);
      NotifyOfPropertyChange(() => CanDuplicateSelected);
      NotifyOfPropertyChange(() => CanRemoveSelected);
    }

    public void HandleKeyInput(Key key)
    {
      var gesture = string.Format("{0}{1}{2}{3}{4}",
                                  HasModifier(ModifierKeys.Control, "Ctrl"),
                                  HasModifier(ModifierKeys.Shift, "Shift"),
                                  HasModifier(ModifierKeys.Alt, "Alt"),
                                  HasModifier(ModifierKeys.Windows, "Win"),
                                  key);

      //Console.WriteLine("Gesture: {0}", gesture);

      if (_keyBindings.ContainsKey(gesture))
      {
        _keyBindings[gesture].ExecuteIfPossible();
      }
    }

    private static string HasModifier(ModifierKeys key, string text)
    {
      return Keyboard.Modifiers.HasFlag(key)
               ? string.Format("{0}+", text)
               : string.Empty;
    }

    private void SetupKeyBindings()
    {
      _keyBindings = new Dictionary<string, KeyBinding>
                     {
                       {
                         "Delete", new KeyBinding(() => true, DeleteSelectedElements)
                       },
                       {
                         "Ctrl+A", new KeyBinding(() => true, SelectAllNodes)
                       },
                       {
                         "Up", new KeyBinding(() => true, () => MoveSelectedItems(Direction.Up))
                       },
                       {
                         "Down", new KeyBinding(() => true, () => MoveSelectedItems(Direction.Down))
                       },
                       {
                         "Left", new KeyBinding(() => true, () => MoveSelectedItems(Direction.Left))
                       },
                       {
                         "Right", new KeyBinding(() => true, () => MoveSelectedItems(Direction.Right))
                       }
                     };
    }

    private void DeleteSelectedElements()
    {
      Console.WriteLine("Delete selection!");
    }

    private void SelectAllNodes()
    {
      Console.WriteLine("select all!");
    }

    private void MoveSelectedItems(Direction direction)
    {
      Console.WriteLine("Move selection {0}!", direction);
    }
  }
}