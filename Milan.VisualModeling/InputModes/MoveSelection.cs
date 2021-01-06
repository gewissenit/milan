#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Emporer.WPF;
using Milan.VisualModeling.ViewModels;
using ReactiveUI;

namespace Milan.VisualModeling.InputModes
{
  public class MoveSelection : InputMode
  {
    private const double PanningMargin = 20;
    private const int AutoMoveDelta = 5;

    private readonly Direction[] _horizontalDirections =
    {
      Direction.Left, Direction.Right
    };

    private readonly Direction[] _verticalDirections =
    {
      Direction.Left, Direction.Right
    };

    private IDisposable _autoMoveSubscription;
    private Point _currentMousePosition;
    private IDisposable _mouseLeftUpSubscription;
    private IDisposable _moveSelectedItemsSubscription;
    private IDisposable _moveVisibleAreaSignalSubscription;
    private Rect _selectionBox;

    public MoveSelection(VisualEditor visualEditor)
      : base(visualEditor)
    {
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
      _sw =Stopwatch.StartNew();
      base.Enter();
      VisualEditor.ChangeCursor(Cursors.SizeAll);

      VisualEditor.CaptureMouse();

      var movingDelta = VisualEditor.MouseMoved.Zip(VisualEditor.MouseMoved.Skip(1), (p, c) => new Vector(c.X - p.X, c.Y - p.Y));

      var autoMoveSignal = Observable.Interval(TimeSpan.FromMilliseconds(10))
                                     .ObserveOnDispatcher();

      _autoMoveSubscription = autoMoveSignal.Subscribe();
      _moveVisibleAreaSignalSubscription = autoMoveSignal.CombineLatest(VisualEditor.MouseMoved, (t, p) => p)
                                                         .Select(ToDirectionalDelta)
                                                         .Where(IsNearBorder)
                                                         .Subscribe(MoveVisibleArea);

      _moveSelectedItemsSubscription = movingDelta.Subscribe(MoveSelectedItems);
      _mouseLeftUpSubscription = VisualEditor.MouseLeftUp.Subscribe(SwitchToSelectMode);
      Console.WriteLine("Init took {0}", _sw.ElapsedMilliseconds);
    }

    private Stopwatch _sw;

    private static bool IsNearBorder(DirectionalDelta delta)
    {
      return delta != null;
    }

    private DirectionalDelta ToDirectionalDelta(Point mousePosition)
    {
      if (mousePosition.X < VisualEditor.ViewPortLeft + PanningMargin)
      {
        return new DirectionalDelta(Direction.Left, AutoMoveDelta);
      }
      if (mousePosition.X > VisualEditor.ViewPortRight - PanningMargin)
      {
        return new DirectionalDelta(Direction.Right, AutoMoveDelta);
      }
      if (mousePosition.Y < VisualEditor.ViewPortTop + PanningMargin)
      {
        return new DirectionalDelta(Direction.Up, AutoMoveDelta);
      }
      if (mousePosition.Y > VisualEditor.ViewPortBottom - PanningMargin)
      {
        return new DirectionalDelta(Direction.Down, AutoMoveDelta);
      }
      return null;
    }

    private void MoveVisibleArea(DirectionalDelta delta)
    {
      VisualEditor.Scroll(delta);
    }

    public override void Exit()
    {
      base.Exit();
      VisualEditor.UpdateCanvasSize();
      VisualEditor.ReleaseMouseCapture();
      _moveSelectedItemsSubscription.Dispose();
      _mouseLeftUpSubscription.Dispose();
      _moveVisibleAreaSignalSubscription.Dispose();
      _autoMoveSubscription.Dispose();
    }

    private void MoveSelectedItems(Vector delta)
    {
      var beganMovementAfter = _sw.ElapsedMilliseconds;

      var selectedMovables = GetSelectedMovables()
        .ToArray();

      var gotSelectedMovablesAfter = _sw.ElapsedMilliseconds;

      if (selectedMovables.Any(x => !x.CanMoveBy(delta)))
      {
        return;
      }

      var checkedMovementAfter = _sw.ElapsedMilliseconds;

      foreach (var movableSelectedItem in selectedMovables)
      {
        movableSelectedItem.MoveBy(delta);
      }

      if (_sw.IsRunning)
      {
        var format = @"began                 {0}
got selected movables {1}
checked movement      {2}
moved all             {3}";

        Console.WriteLine(format, beganMovementAfter, gotSelectedMovablesAfter, checkedMovementAfter, _sw.ElapsedMilliseconds);
        _sw.Stop();
      }
    }

    private IEnumerable<IMovable> GetSelectedMovables()
    {
      return VisualEditor.SelectedVisuals.OfType<IMovable>();
    }

    private void SwitchToSelectMode(Point _)
    {
      VisualEditor.ActiveInputMode = new Select(VisualEditor);
    }
  }
}