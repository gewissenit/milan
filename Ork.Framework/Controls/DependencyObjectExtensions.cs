#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Media;

namespace Ork.Framework.Controls
{
  public static class DependencyObjectExtensions
  {
    public static T GetVisualParent<T>(this DependencyObject child) where T : Visual
    {
      while ((child != null) &&
             !(child is T))
      {
        child = VisualTreeHelper.GetParent(child);
      }
      return child as T;
    }
  }
}