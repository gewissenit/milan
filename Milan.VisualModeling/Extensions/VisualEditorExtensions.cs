#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Milan.VisualModeling.ViewModels;
using Visual = System.Windows.Media.Visual;

namespace Milan.VisualModeling.Extensions
{
  public static class VisualEditorExtensions
  {
    public static bool ContextOfElementUnderCursorIsNotSelected(Point pointOnCanvas, FrameworkElement element)
    {
      return !ContextOfElementUnderCursorIsSelected(pointOnCanvas, element);
    }

    public static bool ContextOfElementUnderCursorIsSelected(Point pointOnCanvas, FrameworkElement element)
    {
      return IsSelected(GetContentItem(GetItemUnderCursor(pointOnCanvas, element)));
    }

    /// <summary>
    ///   Gets a rectangle surrounding two points.
    /// </summary>
    /// <param name="a">A point.</param>
    /// <param name="b">Another point.</param>
    public static Rect GetBoundingRectangle(Point a, Point b)
    {
      var startX = a.X < b.X
                     ? a.X
                     : b.X;
      var startY = a.Y < b.Y
                     ? a.Y
                     : b.Y;
      var dX = b.X - a.X;
      var dY = b.Y - a.Y;
      var width = Math.Sqrt(dX * dX);
      var height = Math.Sqrt(dY * dY);

      return new Rect(startX, startY, width, height);
    }

    public static object GetContentItem(DependencyObject element)
    {
      var parent = element;

      while (parent != null)
      {
        var fe = parent as FrameworkElement;
        if (IsContentElement(fe))
        {
          return fe.DataContext;
        }
        parent = VisualTreeHelper.GetParent(parent);
      }

      return null;
    }

    public static DependencyObject GetItemUnderCursor(Point pointOnCanvas, FrameworkElement element)
    {
      var hitTest = HitTest(element, pointOnCanvas);
      return hitTest == null
               ? null
               : hitTest.VisualHit;
    }

    public static Point GetPosition(MouseEventArgs e, FrameworkElement element)
    {
      return e.GetPosition(element);
    }


    // The BCLs VisualTreeHelper.HitTest method does not pay attention to the IsHitTestVisible property.
    // This helper does.
    public static HitTestResult HitTest(Visual visual, Point point)
    {
      // This 'HitTest' method also takes the 'IsHitTestVisible' and 'IsVisible' properties
      // into account, so use it instead of the normal VisualTreeHelper.HitTest instead!
      HitTestResult result = null;

      // Use the advanced HitTest method and specify a custom filter that filters out the
      // invisible elements or the elements that don't allow hittesting.
      VisualTreeHelper.HitTest(visual,
                               (target) =>
                               {
                                 var uiElement = target as UIElement;
                                 if ((uiElement != null) &&
                                     (!uiElement.IsHitTestVisible || !uiElement.IsVisible))
                                 {
                                   return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                                 }
                                 return HitTestFilterBehavior.Continue;
                               },
                               (target) =>
                               {
                                 result = target;
                                 return HitTestResultBehavior.Stop;
                               },
                               new PointHitTestParameters(point));

      return result;
    }


    public static T GetItemOfTypeAt<T>(Point location, FrameworkElement element) where T : class
    {
      var somethingUnderCursor = GetItemUnderCursor(location, element);

      if (somethingUnderCursor == null)
      {
        return null;
      }

      var frameworkElement = somethingUnderCursor as FrameworkElement;

      if (frameworkElement == null)
      {
        return null;
      }

      if (frameworkElement.DataContext == null)
      {
        return null;
      }

      return frameworkElement.DataContext as T;
    }

    public static ISelectable GetSelectableAt(Point location, FrameworkElement element)
    {
      return GetItemOfTypeAt<ISelectable>(location, element);
    }

    public static IVisual GetVisualAt(Point location, FrameworkElement element)
    {
      var somethingUnderCursor = GetItemUnderCursor(location, element);

      if (somethingUnderCursor == null)
      {
        return null;
      }

      var frameworkElement = somethingUnderCursor as FrameworkElement;

      if (frameworkElement == null)
      {
        return null;
      }

      if (frameworkElement.DataContext == null)
      {
        return null;
      }

      return frameworkElement.DataContext as IVisual;
    }


    public static bool IsContainedIn(this IVisual visual, Rect area)
    {
      return area.Contains(visual.Bounds);
    }

    public static bool IsContentElement(FrameworkElement element)
    {
      if (element == null)
      {
        return false;
      }

      if (!(element is ContentPresenter))
      {
        return false;
      }
      // bad ugly uber-hack
      return element.Parent()
                    .Parent()
                    .Parent()
                    .Parent()
                    .Parent()
                    .Name == "Canvas";
    }

    public static bool IsSelected(object arg)
    {
      var visual = arg as ISelectable;
      return visual != null && visual.IsSelected;
    }


    public static bool OverItemOfType<TItemType>(Point location, FrameworkElement element) where TItemType : class
    {
      return GetItemOfTypeAt<TItemType>(location, element) != null;
    }

    public static bool NotOverSelectable(Point location, FrameworkElement element)
    {
      return !OverSelectable(location, element);
    }

    public static bool OverSelectable(Point location, FrameworkElement element)
    {
      var selectable = GetSelectableAt(location, element);
      return selectable != null;
    }

    public static bool OverSelected(Point location, FrameworkElement element)
    {
      var selectable = GetSelectableAt(location, element);
      if (selectable == null)
      {
        return false;
      }
      return selectable.IsSelected;
    }

    public static bool OverUnselected(Point location, FrameworkElement element)
    {
      var selectable = GetSelectableAt(location, element);
      if (selectable == null)
      {
        return false;
      }
      return !selectable.IsSelected;
    }


    public static FrameworkElement Parent(this DependencyObject element)
    {
      var parent = VisualTreeHelper.GetParent(element);
      return (FrameworkElement) parent;
    }

    public static void SelectItem(this VisualEditor editor, ISelectable item)
    {
      SelectItems(editor,
                  new[]
                  {
                    item
                  });
    }

    public static void SelectItems(this VisualEditor editor, IEnumerable<ISelectable> items)
    {
      foreach (var selectable in items)
      {
        selectable.IsSelected = true;
      }

      editor.SelectedItems = items;
    }

    public static void ToggleIsSelected(this VisualEditor editor, ISelectable item)
    {
      SelectItem(editor, item);
    }

    public static void SelectItems(this VisualEditor editor, IEnumerable<object> items)
    {
      SelectItems(editor, items.OfType<ISelectable>());
    }

    private static string Tabs(string text, uint defaultSpace)
    {
      var count = defaultSpace - text.Length;
      var tabs = string.Empty;
      for (var i = 0; i < count; i++)
      {
        tabs += "_";
      }
      return tabs;
    }
  }
}