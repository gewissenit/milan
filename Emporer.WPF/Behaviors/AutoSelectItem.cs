#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Emporer.WPF.Behaviors
{
  public class AutoSelectItem : Behavior<ComboBox>
  {
    protected override void OnAttached()
    {
      RegisterCollectionChangedListener();
      base.OnAttached();
    }


    private void RegisterCollectionChangedListener()
    {
      var collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
      if (collection == null)
      {
        return;
      }

      collection.CollectionChanged += SelectAvailableItem;
    }

    private void UnregisterCollectionChangedListener()
    {
      var collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
      if (collection == null)
      {
        return;
      }

      collection.CollectionChanged -= SelectAvailableItem;
    }

    protected override void OnDetaching()
    {
      UnregisterCollectionChangedListener();
    }

    private void SelectAvailableItem(object sender, NotifyCollectionChangedEventArgs e)
    {
      SelectAvailableItem();
    }

    private void SelectAvailableItem()
    {
      AssociatedObject.SelectedIndex = 0;
    }
  }
}