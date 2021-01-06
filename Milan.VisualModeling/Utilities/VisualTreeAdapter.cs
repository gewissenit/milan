using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

//INFO: taken from http://www.scottlogic.com/blog/2010/07/21/exposing-and-binding-to-a-silverlight-scrollviewers-scrollbars.html
namespace Milan.VisualModeling.Utilities
{
  /// <summary>
  ///   Adapts a DependencyObject to provide methods required for generate
  ///   a Linq To Tree API
  /// </summary>
  public class VisualTreeAdapter : ILinqTree<DependencyObject>
  {
    private readonly DependencyObject _item;

    public VisualTreeAdapter(DependencyObject item)
    {
      _item = item;
    }

    public IEnumerable<DependencyObject> Children()
    {
      var childrenCount = VisualTreeHelper.GetChildrenCount(_item);
      for (var i = 0; i < childrenCount; i++)
      {
        yield return VisualTreeHelper.GetChild(_item, i);
      }
    }

    public DependencyObject Parent
    {
      get { return VisualTreeHelper.GetParent(_item); }
    }
  }
}