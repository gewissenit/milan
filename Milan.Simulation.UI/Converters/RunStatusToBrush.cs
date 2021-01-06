#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Milan.Simulation.UI.Converters
{
  public sealed class RunStatusToBrush : DependencyObject, IValueConverter
  {
    public static readonly DependencyProperty NotStartedBrushProperty = DependencyProperty.Register("NotStartedBrush",
                                                                                                    typeof (Brush),
                                                                                                    typeof (RunStatusToBrush),
                                                                                                    new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty RunningBrushProperty = DependencyProperty.Register("RunningBrush",
                                                                                                 typeof (Brush),
                                                                                                 typeof (RunStatusToBrush),
                                                                                                 new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty PausedBrushProperty = DependencyProperty.Register("PausedBrush",
                                                                                                typeof (Brush),
                                                                                                typeof (RunStatusToBrush),
                                                                                                new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty FinishedBrushProperty = DependencyProperty.Register("FinishedBrush",
                                                                                                  typeof (Brush),
                                                                                                  typeof (RunStatusToBrush),
                                                                                                  new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty CanceledBrushProperty = DependencyProperty.Register("CanceledBrush",
                                                                                                  typeof (Brush),
                                                                                                  typeof (RunStatusToBrush),
                                                                                                  new PropertyMetadata(default(Brush)));
    
    public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register("ErrorBrush",
                                                                                                  typeof (Brush),
                                                                                                  typeof (RunStatusToBrush),
                                                                                                  new PropertyMetadata(default(Brush)));
    //todo: apply metro ui style to colors
    public RunStatusToBrush()
    {
      CanceledBrush = new SolidColorBrush(Colors.DarkOrange);
      FinishedBrush = new SolidColorBrush(Colors.Green);
      NotStartedBrush = new SolidColorBrush(Colors.LightGray);
      PausedBrush = new SolidColorBrush(Colors.LightGray);
      RunningBrush = new SolidColorBrush(Colors.Yellow);
      ErrorBrush = new SolidColorBrush(Colors.Red);
    }

    public Brush CanceledBrush
    {
      get { return (Brush) GetValue(CanceledBrushProperty); }
      set { SetValue(CanceledBrushProperty, value); }
    }

    public Brush FinishedBrush
    {
      get { return (Brush) GetValue(FinishedBrushProperty); }
      set { SetValue(FinishedBrushProperty, value); }
    }

    public Brush NotStartedBrush
    {
      get { return (Brush) GetValue(NotStartedBrushProperty); }
      set { SetValue(NotStartedBrushProperty, value); }
    }

    public Brush PausedBrush
    {
      get { return (Brush) GetValue(PausedBrushProperty); }
      set { SetValue(PausedBrushProperty, value); }
    }

    public Brush RunningBrush
    {
      get { return (Brush) GetValue(RunningBrushProperty); }
      set { SetValue(RunningBrushProperty, value); }
    }
    
    public Brush ErrorBrush
    {
      get { return (Brush) GetValue(ErrorBrushProperty); }
      set { SetValue(ErrorBrushProperty, value); }
    }


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is RunStatus))
      {
        return DependencyProperty.UnsetValue;
      }

      var runStatus = (RunStatus) value;

      switch (runStatus)
      {
        case RunStatus.NotStarted:
          return NotStartedBrush;
        case RunStatus.Running:
          return RunningBrush;
        case RunStatus.Paused:
          return PausedBrush;
        case RunStatus.Finished:
          return FinishedBrush;
        case RunStatus.Canceled:
          return CanceledBrush;
        case RunStatus.Error:
          return ErrorBrush;
        default:
          return new SolidColorBrush(Colors.Transparent);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException("RunStatusToBrush can only be used for one way conversion.");
    }
  }
}