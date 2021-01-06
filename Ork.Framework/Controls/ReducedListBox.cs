using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ork.Framework.Controls
{
  /// <summary>
  /// 
  /// </summary>
  public class ReducedListBox : Selector
  {
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new ListBoxItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return false; // forces wrapping of items in a ReducedListBoxItem
    }
  }
}
