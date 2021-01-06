#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Windows;
using System.Windows.Controls;

namespace Ork.Framework.Views
{
  public partial class DialogConductorView : UserControl
  {
    public DialogConductorView()
    {
      InitializeComponent();
    }


    private void DialogConductorView_OnGotFocus(object sender, RoutedEventArgs e)
    {
      Console.WriteLine("Got Focus! {0}", ActiveItem.IsFocused);
    }

    private void ActiveItem_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
    }
  }
}