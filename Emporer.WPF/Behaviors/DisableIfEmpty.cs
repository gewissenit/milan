#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Emporer.WPF.Behaviors
{
  public class DisableIfEmpty : Behavior<ComboBox>
  {
    protected override void OnAttached()
    {
      RegisterCollectionChangedListener();
      DisableIfEmptyItemsSource();
      base.OnAttached();
    }

    private void RegisterCollectionChangedListener()
    {
      var collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
      if (collection == null)
      {
        return;
      }

      collection.CollectionChanged += DisableIfEmptyItemsSource;
    }

    private void UnregisterCollectionChangedListener()
    {
      var collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
      if (collection == null)
      {
        return;
      }

      collection.CollectionChanged -= DisableIfEmptyItemsSource;
    }

    protected override void OnDetaching()
    {
      UnregisterCollectionChangedListener();
    }

    private void DisableIfEmptyItemsSource(object sender, NotifyCollectionChangedEventArgs e)
    {
      DisableIfEmptyItemsSource();
    }

    private void DisableIfEmptyItemsSource()
    {
      if (AssociatedObject.ItemsSource == null)
      {
        AssociatedObject.IsEnabled = false;
      }
      else
      {
        AssociatedObject.IsEnabled = AssociatedObject.ItemsSource.Cast<object>()
                                                     .Any();
      }
    }
  }
}