﻿#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Emporer.WPF.Behaviors
{
  public class PushBinding : FreezableBinding
  {
    #region Dependency Properties

    public static DependencyProperty TargetPropertyMirrorProperty = DependencyProperty.Register("TargetPropertyMirror",
                                                                                                typeof (object),
                                                                                                typeof (PushBinding));

    public static DependencyProperty TargetPropertyListenerProperty = DependencyProperty.Register("TargetPropertyListener",
                                                                                                  typeof (object),
                                                                                                  typeof (PushBinding),
                                                                                                  new UIPropertyMetadata(null,
                                                                                                                         OnTargetPropertyListenerChanged));

    private static void OnTargetPropertyListenerChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var pushBinding = sender as PushBinding;
      pushBinding.TargetPropertyValueChanged();
    }

    #endregion // Dependency Properties

    #region Constructor

    public PushBinding()
    {
      Mode = BindingMode.OneWayToSource;
    }

    #endregion // Constructor

    #region Properties

    public object TargetPropertyMirror
    {
      get { return GetValue(TargetPropertyMirrorProperty); }
      set { SetValue(TargetPropertyMirrorProperty, value); }
    }

    public object TargetPropertyListener
    {
      get { return GetValue(TargetPropertyListenerProperty); }
      set { SetValue(TargetPropertyListenerProperty, value); }
    }

    [DefaultValue(null)]
    public string TargetProperty { get; set; }

    [DefaultValue(null)]
    public DependencyProperty TargetDependencyProperty { get; set; }

    #endregion // Properties

    #region Public Methods

    public void SetupTargetBinding(DependencyObject targetObject)
    {
      if (targetObject == null)
      {
        return;
      }

      // Prevent the designer from reporting exceptions since
      // changes will be made of a Binding in use if it is set
      if (DesignerProperties.GetIsInDesignMode(this))
      {
        return;
      }

      // Bind to the selected TargetProperty, e.g. ActualHeight and get
      // notified about changes in OnTargetPropertyListenerChanged
      var listenerBinding = new Binding
                            {
                              Source = targetObject,
                              Mode = BindingMode.OneWay
                            };
      if (TargetDependencyProperty != null)
      {
        listenerBinding.Path = new PropertyPath(TargetDependencyProperty);
      }
      else
      {
        listenerBinding.Path = new PropertyPath(TargetProperty);
      }
      BindingOperations.SetBinding(this, TargetPropertyListenerProperty, listenerBinding);

      // Set up a OneWayToSource Binding with the Binding declared in Xaml from
      // the Mirror property of this class. The mirror property will be updated
      // everytime the Listener property gets updated
      BindingOperations.SetBinding(this, TargetPropertyMirrorProperty, Binding);

      TargetPropertyValueChanged();
      if (targetObject is FrameworkElement)
      {
        ((FrameworkElement) targetObject).Loaded += delegate
                                                    {
                                                      TargetPropertyValueChanged();
                                                    };
      }
      else if (targetObject is FrameworkContentElement)
      {
        ((FrameworkContentElement) targetObject).Loaded += delegate
                                                           {
                                                             TargetPropertyValueChanged();
                                                           };
      }
    }

    #endregion // Public Methods

    #region Private Methods

    private void TargetPropertyValueChanged()
    {
      var targetPropertyValue = GetValue(TargetPropertyListenerProperty);
      SetValue(TargetPropertyMirrorProperty, targetPropertyValue);
    }

    #endregion // Private Methods

    #region Freezable overrides

    protected override void CloneCore(Freezable sourceFreezable)
    {
      var pushBinding = sourceFreezable as PushBinding;
      TargetProperty = pushBinding.TargetProperty;
      TargetDependencyProperty = pushBinding.TargetDependencyProperty;
      base.CloneCore(sourceFreezable);
    }

    protected override Freezable CreateInstanceCore()
    {
      return new PushBinding();
    }

    #endregion // Freezable overrides
  }
}