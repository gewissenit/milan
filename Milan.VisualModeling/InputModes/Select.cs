#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Milan.VisualModeling.Extensions;
using Milan.VisualModeling.ViewModels;
using ReactiveUI;

namespace Milan.VisualModeling.InputModes
{
  public class Select : InputMode
  {
    private Point _currentMousePosition;
    private bool _isSpanningSelection;
    private Rect _selectionBox;
    private Point _selectionStartPoint;
    private IDisposable _spanningSelectionSubscription;
    private IDisposable _startedMovingSelectionSubscription;
    private IDisposable _startedMovingUnselectedSubscription;
    private IDisposable _startedSpanningSelectionSubscription;
    private IDisposable _stoppedSpanningSelectionSubscription;
    private IDisposable _updateMousePositionSubscription;

    public Select(VisualEditor visualEditor)
      : base(visualEditor)
    {
    }

    public bool IsSpanningSelection
    {
      get { return _isSpanningSelection; }
      private set { this.RaiseAndSetIfChanged(ref _isSpanningSelection, value); }
    }

    public Rect SelectionBox
    {
      get { return _selectionBox; }
      set { this.RaiseAndSetIfChanged(ref _selectionBox, value); }
    }

    public Point CurrentMousePosition
    {
      get { return _currentMousePosition; }
      set { this.RaiseAndSetIfChanged(ref _currentMousePosition, value); }
    }

    public override void Enter()
    {
      _updateMousePositionSubscription = VisualEditor.MouseMoved.Subscribe(p => CurrentMousePosition = p);

      var clickedUnselected = VisualEditor.MouseLeftDown.Where(
                                                               x =>
                                                               MouseIsInsideViewPort(x) &&
                                                               VisualEditorExtensions.OverUnselected(x, VisualEditor.CanvasControl))
                                          .Select(x => VisualEditorExtensions.GetVisualAt(x, VisualEditor.CanvasControl));

      var clickedSelection =
        VisualEditor.MouseLeftDown.Where(x => MouseIsInsideViewPort(x) && VisualEditorExtensions.OverSelected(x, VisualEditor.CanvasControl));

      var startedSpanningSelection = VisualEditor.MouseLeftDown.Where(MouseIsInsideViewPort)
                                                 .Where(MouseIsNotOverSomethingSelectable);


      _stoppedSpanningSelectionSubscription = VisualEditor.MouseLeftUp.Subscribe(StopSpanningSelection);

      _startedMovingSelectionSubscription = clickedSelection.Join(VisualEditor.MouseMoved,
                                                                  v => VisualEditor.MouseLeftUp,
                                                                  v => VisualEditor.MouseLeftUp,
                                                                  (start, movement) => start)
                                                            .Subscribe(_ => SwitchToMoveSelectionMode());

      _startedMovingUnselectedSubscription = clickedUnselected.GroupJoin(VisualEditor.MouseMoved,
                                                                         v => VisualEditor.MouseLeftUp,
                                                                         v => VisualEditor.MouseLeftUp,
                                                                         (unselected, _) => unselected)
                                                              .Subscribe(visual =>
                                                                         {
                                                                           var movable = visual as IMovable;
                                                                           if (movable != null)
                                                                           {
                                                                             SwitchToMoveSelectionMode();
                                                                           }
                                                                           var selectable = visual as ISelectable;
                                                                           if (selectable != null)
                                                                           {
                                                                             VisualEditor.ClearSelection();
                                                                             VisualEditor.SelectItem(selectable);
                                                                           }
                                                                         });

      _startedSpanningSelectionSubscription = startedSpanningSelection.Subscribe(StartSpanningSelection);
    }

    private bool MouseIsNotOverSomethingSelectable(Point point)
    {
      return VisualEditorExtensions.NotOverSelectable(point, VisualEditor.CanvasControl);
    }

    protected bool MouseIsInsideViewPort(Point point)
    {
      if (point.X < VisualEditor.ViewPortLeft)
      {
        return false;
      }

      if (point.Y < VisualEditor.ViewPortTop)
      {
        return false;
      }

      if (point.X > VisualEditor.ViewPortRight)
      {
        return false;
      }

      if (point.Y > VisualEditor.ViewPortBottom)
      {
        return false;
      }

      return true;
    }

    public override void Exit()
    {
      base.Exit();
      VisualEditor.ReleaseMouseCapture();
      DisposeIfNotNull(_spanningSelectionSubscription);
      DisposeIfNotNull(_startedMovingSelectionSubscription);
      DisposeIfNotNull(_startedMovingUnselectedSubscription);
      DisposeIfNotNull(_stoppedSpanningSelectionSubscription);
      DisposeIfNotNull(_startedSpanningSelectionSubscription);
      DisposeIfNotNull(_updateMousePositionSubscription);
    }

    private void DisposeIfNotNull(IDisposable disposable)
    {
      if (disposable == null)
      {
        return;
      }
      disposable.Dispose();
    }

    private void SpanSelection(Point currentMouseLocation)
    {
      SelectionBox = VisualEditorExtensions.GetBoundingRectangle(_selectionStartPoint, currentMouseLocation);
    }

    private void StartSpanningSelection(Point startPoint)
    {
      IsSpanningSelection = true;
      VisualEditor.CaptureMouse();
      _selectionStartPoint = startPoint;
      _spanningSelectionSubscription = VisualEditor.MouseMoved.Subscribe(SpanSelection);
    }

    private void StopSpanningSelection(Point currentMouseLocation)
    {
      if (!IsSpanningSelection)
      {
        return;
      }

      IsSpanningSelection = false;
      VisualEditor.ReleaseMouseCapture();

      if (_spanningSelectionSubscription == null)
      {
        return;
      }

      _spanningSelectionSubscription.Dispose();

      VisualEditor.ClearSelection();

      var itemsToSelect = VisualEditor.Visuals.Where(x => SelectionBox.Contains(x.Bounds))
                                      .OfType<INode>() // do not select edges via multiselect
                                      .Where(x => !x.IsSelected)
                                      .ToArray();
      VisualEditor.SelectItems(itemsToSelect);
      
      SelectionBox = new Rect();
    }

    private void SwitchToMoveSelectionMode()
    {
      VisualEditor.ActiveInputMode = new MoveSelection(VisualEditor);
    }
  }
}