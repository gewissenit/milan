#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Emporer.WPF.Behaviors
{
  public class BindableSelectedTreeViewItemBehavior : Behavior<TreeView>
  {
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
                                                                                                 typeof (object),
                                                                                                 typeof (BindableSelectedTreeViewItemBehavior),
                                                                                                 new UIPropertyMetadata(null, OnSelectedItemChanged));

    public object SelectedItem
    {
      get { return GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      if (AssociatedObject != null)
      {
        AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
      }
    }

    private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var treeView = e.NewValue as TreeViewItem;
      if (treeView != null)
      {
        treeView.SetValue(TreeViewItem.IsSelectedProperty, true);
      }
    }

    private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      SelectedItem = e.NewValue;
    }
  }
}