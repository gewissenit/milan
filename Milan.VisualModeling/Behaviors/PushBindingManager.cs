#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;

namespace Milan.VisualModeling.Behaviors
{
  public class PushBindingManager
  {
    public static DependencyProperty PushBindingsProperty = DependencyProperty.RegisterAttached("PushBindingsInternal",
                                                                                                typeof (PushBindingCollection),
                                                                                                typeof (PushBindingManager),
                                                                                                new UIPropertyMetadata(null));

    public static DependencyProperty StylePushBindingsProperty = DependencyProperty.RegisterAttached("StylePushBindings",
                                                                                                     typeof (PushBindingCollection),
                                                                                                     typeof (PushBindingManager),
                                                                                                     new UIPropertyMetadata(null,
                                                                                                                            StylePushBindingsChanged));

    public static PushBindingCollection GetPushBindings(DependencyObject obj)
    {
      if (obj.GetValue(PushBindingsProperty) == null)
      {
        obj.SetValue(PushBindingsProperty, new PushBindingCollection(obj));
      }
      return (PushBindingCollection) obj.GetValue(PushBindingsProperty);
    }


    public static PushBindingCollection GetStylePushBindings(DependencyObject obj)
    {
      return (PushBindingCollection) obj.GetValue(StylePushBindingsProperty);
    }

    public static void SetPushBindings(DependencyObject obj, PushBindingCollection value)
    {
      obj.SetValue(PushBindingsProperty, value);
    }

    public static void SetStylePushBindings(DependencyObject obj, PushBindingCollection value)
    {
      obj.SetValue(StylePushBindingsProperty, value);
    }

    public static void StylePushBindingsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      if (target != null)
      {
        var stylePushBindings = e.NewValue as PushBindingCollection;
        var pushBindingCollection = GetPushBindings(target);
        foreach (var pushBinding in stylePushBindings)
        {
          var pushBindingClone = pushBinding.Clone() as PushBinding;
          pushBindingCollection.Add(pushBindingClone);
        }
      }
    }
  }
}