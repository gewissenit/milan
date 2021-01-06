#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;

namespace Milan.Simulation.ShiftSystems.UI.Controls
{
  /// <summary>
  ///   Interaction logic for TimeAxisDay.xaml
  /// </summary>
  public partial class TimeAxisDay
  {
    public static readonly DependencyProperty ShowDayNumbersProperty = DependencyProperty.Register("ShowDayNumbers",
                                                                                                   typeof (bool),
                                                                                                   typeof (TimeAxisDay),
                                                                                                   new PropertyMetadata(true));

    public static readonly DependencyProperty ShowDayChangeIndicatorsProperty = DependencyProperty.Register("ShowDayChangeIndicators",
                                                                                                            typeof (bool),
                                                                                                            typeof (TimeAxisDay),
                                                                                                            new PropertyMetadata(true));

    public TimeAxisDay()
    {
      InitializeComponent();
    }

    public bool ShowDayNumbers
    {
      get { return (bool) GetValue(ShowDayNumbersProperty); }
      set { SetValue(ShowDayNumbersProperty, value); }
    }

    public bool ShowDayChangeIndicators
    {
      get { return (bool) GetValue(ShowDayChangeIndicatorsProperty); }
      set { SetValue(ShowDayChangeIndicatorsProperty, value); }
    }
  }
}