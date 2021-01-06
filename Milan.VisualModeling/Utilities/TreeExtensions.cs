using System.Collections.Generic;
using System.Linq;
using System.Windows;

//INFO: taken from http://www.scottlogic.com/blog/2010/07/21/exposing-and-binding-to-a-silverlight-scrollviewers-scrollbars.html
namespace Milan.VisualModeling.Utilities
{
  public static class TreeExtensions
  {
    /// <summary>
    ///   Returns a collection of descendant elements.
    /// </summary>
    public static IEnumerable<DependencyObject> Descendants(this DependencyObject item)
    {
      ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);
      foreach (var child in adapter.Children())
      {
        yield return child;

        foreach (var grandChild in child.Descendants())
        {
          yield return grandChild;
        }
      }
    }

    /// <summary>
    ///   Returns a collection containing this element and all descendant elements.
    /// </summary>
    public static IEnumerable<DependencyObject> DescendantsAndSelf(this DependencyObject item)
    {
      yield return item;

      foreach (var child in item.Descendants())
      {
        yield return child;
      }
    }

    /// <summary>
    ///   Returns a collection of ancestor elements.
    /// </summary>
    public static IEnumerable<DependencyObject> Ancestors(this DependencyObject item)
    {
      ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);

      var parent = adapter.Parent;
      while (parent != null)
      {
        yield return parent;
        adapter = new VisualTreeAdapter(parent);
        parent = adapter.Parent;
      }
    }

    /// <summary>
    ///   Returns a collection containing this element and all ancestor elements.
    /// </summary>
    public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject item)
    {
      yield return item;

      foreach (var ancestor in item.Ancestors())
      {
        yield return ancestor;
      }
    }

    /// <summary>
    ///   Returns a collection of child elements.
    /// </summary>
    public static IEnumerable<DependencyObject> Elements(this DependencyObject item)
    {
      ILinqTree<DependencyObject> adapter = new VisualTreeAdapter(item);
      return adapter.Children();
    }

    /// <summary>
    ///   Returns a collection of the sibling elements before this node, in document order.
    /// </summary>
    public static IEnumerable<DependencyObject> ElementsBeforeSelf(this DependencyObject item)
    {
      if (item.Ancestors()
              .FirstOrDefault() == null)
      {
        yield break;
      }
      foreach (var child in item.Ancestors()
                                .First()
                                .Elements().TakeWhile(child => !child.Equals(item)))
      {
        yield return child;
      }
    }

    /// <summary>
    ///   Returns a collection of the after elements after this node, in document order.
    /// </summary>
    public static IEnumerable<DependencyObject> ElementsAfterSelf(this DependencyObject item)
    {
      if (item.Ancestors()
              .FirstOrDefault() == null)
      {
        yield break;
      }
      var afterSelf = false;
      foreach (var child in item.Ancestors()
                                .First()
                                .Elements())
      {
        if (afterSelf)
        {
          yield return child;
        }

        if (child.Equals(item))
        {
          afterSelf = true;
        }
      }
    }

    /// <summary>
    ///   Returns a collection containing this element and all child elements.
    /// </summary>
    public static IEnumerable<DependencyObject> ElementsAndSelf(this DependencyObject item)
    {
      yield return item;

      foreach (var child in item.Elements())
      {
        yield return child;
      }
    }

    /// <summary>
    ///   Returns a collection of descendant elements which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> Descendants<T>(this DependencyObject item)
    {
      return item.Descendants()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection of the sibling elements before this node, in document order
    ///   which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> ElementsBeforeSelf<T>(this DependencyObject item)
    {
      return item.ElementsBeforeSelf()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection of the after elements after this node, in document order
    ///   which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> ElementsAfterSelf<T>(this DependencyObject item)
    {
      return item.ElementsAfterSelf()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection containing this element and all descendant elements
    ///   which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this DependencyObject item)
    {
      return item.DescendantsAndSelf()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection of ancestor elements which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> Ancestors<T>(this DependencyObject item)
    {
      return item.Ancestors()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection containing this element and all ancestor elements
    ///   which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this DependencyObject item)
    {
      return item.AncestorsAndSelf()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection of child elements which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> Elements<T>(this DependencyObject item)
    {
      return item.Elements()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }

    /// <summary>
    ///   Returns a collection containing this element and all child elements.
    ///   which match the given type.
    /// </summary>
    public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this DependencyObject item)
    {
      return item.ElementsAndSelf()
                 .Where(i => i is T)
                 .Cast<DependencyObject>();
    }
  }
}