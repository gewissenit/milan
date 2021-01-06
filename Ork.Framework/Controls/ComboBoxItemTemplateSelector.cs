#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Controls;

namespace Ork.Framework.Controls
{
  public class ComboBoxItemTemplateSelector : DataTemplateSelector
  {
    public DataTemplate DropDownTemplate { get; set; }
    public DataTemplate SelectedTemplate { get; set; }


    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var comboBoxItem = container.GetVisualParent<ComboBoxItem>();
      if (comboBoxItem == null)
      {
        return SelectedTemplate;
      }
      return DropDownTemplate;
    }
  }
}