#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Controls;

namespace Emporer.Math.Distribution.UI.Controls
{
  public partial class DistributionSelector : UserControl
  {
    public static readonly DependencyProperty ValueTemplateProperty = DependencyProperty.Register("ValueTemplate",
                                                                                                  typeof (DataTemplate),
                                                                                                  typeof (DistributionSelector),
                                                                                                  new PropertyMetadata(default(DataTemplate)));

    public DistributionSelector()
    {
      InitializeComponent();
    }

    public DataTemplate ValueTemplate
    {
      get { return (DataTemplate) GetValue(ValueTemplateProperty); }
      set { SetValue(ValueTemplateProperty, value); }
    }
  }
}