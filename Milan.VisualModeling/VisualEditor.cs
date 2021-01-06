using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Emporer.WPF;
using Milan.VisualModeling.Extensions;
using Milan.VisualModeling.InputModes;
using Milan.VisualModeling.ViewModels;
using ReactiveUI;

namespace Milan.VisualModeling
{
  [TemplatePart(Name = "PART_ViewPort", Type = typeof (ScrollViewer))]
  [TemplatePart(Name = "PART_Canvas", Type = typeof (Canvas))]
  public class VisualEditor : Control, INotifyPropertyChanged
  {
    private const double MarginToCanvasBounds = 0;

    public static readonly DependencyProperty MoveCommandProperty = DependencyProperty.Register("MoveCommand",
                                                                                                typeof (ICommand),
                                                                                                typeof (VisualEditor),
                                                                                                new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register("SelectCommand",
                                                                                                  typeof (ICommand),
                                                                                                  typeof (VisualEditor),
                                                                                                  new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty NodesProperty = DependencyProperty.Register("Nodes",
                                                                                          typeof (ObservableCollection<INode>),
                                                                                          typeof (VisualEditor),
                                                                                          new PropertyMetadata(default(ObservableCollection<INode>),
                                                                                                               OnNodesChanged));


    public static readonly DependencyProperty EdgesProperty = DependencyProperty.Register("Edges",
                                                                                          typeof (ObservableCollection<IEdge>),
                                                                                          typeof (VisualEditor),
                                                                                          new PropertyMetadata(default(ObservableCollection<IEdge>)));

    public static readonly DependencyProperty ActiveInputModeProperty = DependencyProperty.Register("ActiveInputMode",
                                                                                                    typeof (IInputMode),
                                                                                                    typeof (VisualEditor),
                                                                                                    new PropertyMetadata(default(IInputMode),
                                                                                                                         UpdateActiveInputMode));


    public static readonly DependencyProperty TokensProperty = DependencyProperty.Register("Tokens",
                                                                                           typeof (ObservableCollection<IToken>),
                                                                                           typeof (VisualEditor),
                                                                                           new PropertyMetadata(default(ObservableCollection<IToken>)));

    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems",
                                                                                                  typeof (IEnumerable<object>),
                                                                                                  typeof (VisualEditor),
                                                                                                  new PropertyMetadata(default(IEnumerable<object>)));

    public static readonly DependencyProperty CanvasProperty = DependencyProperty.Register("Canvas",
                                                                                           typeof (Rect),
                                                                                           typeof (VisualEditor),
                                                                                           new PropertyMetadata(default(Rect)));

    public static readonly DependencyProperty ViewPortTopProperty = DependencyProperty.Register("ViewPortTop",
                                                                                                typeof (double),
                                                                                                typeof (VisualEditor),
                                                                                                new PropertyMetadata(default(double)));

    public static readonly DependencyProperty ViewPortBottomProperty = DependencyProperty.Register("ViewPortBottom",
                                                                                                   typeof (double),
                                                                                                   typeof (VisualEditor),
                                                                                                   new PropertyMetadata(default(double)));

    public static readonly DependencyProperty ViewPortLeftProperty = DependencyProperty.Register("ViewPortLeft",
                                                                                                 typeof (double),
                                                                                                 typeof (VisualEditor),
                                                                                                 new PropertyMetadata(default(double)));

    public static readonly DependencyProperty ViewPortRightProperty = DependencyProperty.Register("ViewPortRight",
                                                                                                  typeof (double),
                                                                                                  typeof (VisualEditor),
                                                                                                  new PropertyMetadata(default(double)));

    private readonly Key[] _invalidInputKeys =
    {
      Key.DeadCharProcessed
    };

    private readonly BehaviorSubject<Key[]> _keysPressed;

    private string _pressedKeys;
    private readonly Subject<IEnumerable<ISelectable>> _selectionChanges;

    static VisualEditor()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof (VisualEditor), new FrameworkPropertyMetadata(typeof (VisualEditor)));
    }

    public VisualEditor()
    {
      MouseLeftDown = new Subject<Point>();
      MouseMoved = new Subject<Point>();
      MouseLeftUp = new Subject<Point>();
      _selectionChanges = new Subject<IEnumerable<ISelectable>>();

      _keysPressed = new BehaviorSubject<Key[]>(new Key[0]);
      Loaded += OnLoaded;
    }

    public ScrollViewer ViewPortControl { get; private set; }

    public Canvas CanvasControl { get; private set; }

    public Rect Canvas
    {
      get { return (Rect) GetValue(CanvasProperty); }
      set { SetValue(CanvasProperty, value); }
    }

    public double ViewPortTop
    {
      get { return (double) GetValue(ViewPortTopProperty); }
      set { SetValue(ViewPortTopProperty, value); }
    }

    public double ViewPortBottom
    {
      get { return (double) GetValue(ViewPortBottomProperty); }
      set { SetValue(ViewPortBottomProperty, value); }
    }

    public double ViewPortLeft
    {
      get { return (double) GetValue(ViewPortLeftProperty); }
      set { SetValue(ViewPortLeftProperty, value); }
    }

    public double ViewPortRight
    {
      get { return (double) GetValue(ViewPortRightProperty); }
      set { SetValue(ViewPortRightProperty, value); }
    }

    public IEnumerable<object> SelectedItems
    {
      get { return (IEnumerable<object>) GetValue(SelectedItemsProperty); }
      set
      {
        SetValue(SelectedItemsProperty, value);
        _selectionChanges.OnNext(value.Cast<ISelectable>());
        RaisePropertyChanged("SelectedVisuals");
      }
    }

    public ObservableCollection<INode> Nodes
    {
      get { return (ObservableCollection<INode>) GetValue(NodesProperty); }
      set { SetValue(NodesProperty, value); }
    }

    public ObservableCollection<IEdge> Edges
    {
      get { return (ObservableCollection<IEdge>) GetValue(EdgesProperty); }
      set { SetValue(EdgesProperty, value); }
    }

    public ObservableCollection<IToken> Tokens
    {
      get { return (ObservableCollection<IToken>) GetValue(TokensProperty); }
      set { SetValue(TokensProperty, value); }
    }

    public IInputMode ActiveInputMode
    {
      get { return (IInputMode) GetValue(ActiveInputModeProperty); }
      set { SetValue(ActiveInputModeProperty, value); }
    }

    public Subject<Point> MouseLeftDown { get; private set; }

    public Subject<Point> MouseLeftUp { get; private set; }

    public Subject<Point> MouseMoved { get; private set; }

    public string PressedKeys
    {
      get { return _pressedKeys; }
      set
      {
        _pressedKeys = value;
        RaisePropertyChanged();
      }
    }

    public IEnumerable<ISelectable> SelectedVisuals
    {
      get
      {
        return Visuals.OfType<ISelectable>()
                      .Where(v => v.IsSelected);
      }
    }

    public IObservable<IEnumerable<ISelectable>> SelectionChanges
    {
      get { return _selectionChanges; }
    }

    public IEnumerable<IVisual> Visuals
    {
      get
      {
        if (Nodes != null)
        {
          foreach (var node in Nodes)
          {
            yield return node;
          }
        }

        if (Edges != null)
        {
          foreach (var edge in Edges)
          {
            yield return edge;
          }
        }

        if (Tokens != null)
        {
          foreach (var token in Tokens)
          {
            yield return token;
          }
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override void OnApplyTemplate()
    {
      ViewPortControl = GetTemplateChild("PART_ViewPort") as ScrollViewer;
      CanvasControl = GetTemplateChild("PART_Canvas") as Canvas;
      InitializeViewLogic();
    }

    private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var editor = (VisualEditor) d;

      if (e.OldValue != null)
      {
        var oc = (ObservableCollection<INode>) e.OldValue;
        oc.CollectionChanged -= editor.Nodes_CollectionChanged;
      }

      editor.Nodes.CollectionChanged += editor.Nodes_CollectionChanged;
    }

    private static void InputModesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var editor = (VisualEditor) d;

      // when the InputModes change, select the 1st one as new active one.
      editor.ActiveInputMode = ((IEnumerable<IInputMode>) e.NewValue).FirstOrDefault();
    }

    private static void UpdateActiveInputMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var editor = (VisualEditor) d;

      if (e.OldValue != null)
      {
        ((IInputMode) e.OldValue).Exit();
      }

      editor.ActiveInputMode.Enter();
    }

    private void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdateCanvasSize();
    }

    public void UpdateCanvasSize()
    {
      if (!Nodes.Any())
      {
        Canvas = new Rect(0, 0, ActualWidth, ActualHeight);
        return;
      }

      Canvas = new Rect(0, 0, Nodes.Max(x => x.Bounds.Right) + MarginToCanvasBounds, Nodes.Max(x => x.Bounds.Bottom) + MarginToCanvasBounds);
    }

    public static void ChangeCursor(Cursor cursor)
    {
      Mouse.OverrideCursor = cursor;
    }

    public void ClearSelection()
    {
      foreach (var selectedItem in Visuals.OfType<ISelectable>()
                                          .Where(x => x.IsSelected)
                                          .ToArray())
      {
        selectedItem.IsSelected = false;
      }

      RaisePropertyChanged("SelectedVisuals");
    }

    private void InitializeViewLogic()
    {
      ActiveInputMode = new Select(this);

      //TODO: remove debug stuff:
      _keysPressed.Select(x => x.Aggregate(string.Empty, (current, key) => current + string.Format("{0} ", key)))
                  .Subscribe(x =>
                             {
                               PressedKeys = x;
                               HandleKeyboardInput(x);
                             });

      ViewPortControl.ScrollChanged += (s, e) => UpdateViewPort(e.HorizontalOffset, e.VerticalOffset, e.ViewportWidth, e.ViewportHeight);

      UpdateViewPort(ViewPortControl.ContentHorizontalOffset,
                     ViewPortControl.ContentVerticalOffset,
                     ViewPortControl.ViewportWidth,
                     ViewPortControl.ViewportHeight);
    }

    private void HandleKeyboardInput(string s)
    {
      Console.WriteLine(s);
    }

    public void Scroll(DirectionalDelta delta)
    {
      switch (delta.Direction)
      {
        case Direction.Left:
          ViewPortControl.ScrollToHorizontalOffset(ViewPortLeft - delta.Distance);
          return;
        case Direction.Right:
          ViewPortControl.ScrollToHorizontalOffset(ViewPortLeft + delta.Distance);
          return;
        case Direction.Up:
          ViewPortControl.ScrollToVerticalOffset(ViewPortTop - delta.Distance);
          return;
        case Direction.Down:
          ViewPortControl.ScrollToVerticalOffset(ViewPortTop + delta.Distance);
          return;
      }
    }

    private void UpdateViewPort(double left, double top, double width, double height)
    {
      ViewPortLeft = left;
      ViewPortRight = left + width;
      ViewPortTop = top;
      ViewPortBottom = top + height;
    }

    private bool IsAValidInputKey(Key key)
    {
      return !_invalidInputKeys.Contains(key);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      MouseMoved.OnNext(VisualEditorExtensions.GetPosition(e, CanvasControl));
      base.OnMouseMove(e);
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      MouseLeftUp.OnNext(VisualEditorExtensions.GetPosition(e, CanvasControl));
      base.OnMouseLeftButtonUp(e);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      MouseLeftDown.OnNext(VisualEditorExtensions.GetPosition(e, CanvasControl));
      base.OnMouseLeftButtonDown(e);
    }
  }
}