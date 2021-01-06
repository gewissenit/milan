using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using MahApps.Metro.Controls;
using Emporer.WPF.Converter;
using System.Globalization;
using System.Windows.Input;
using GeWISSEN.Utils;

namespace Emporer.WPF.Controls
{
  [TemplatePart(Name = ElementHours, Type = typeof(NumericUpDown))]
  [TemplatePart(Name = ElementMinutes, Type = typeof(NumericUpDown))]
  [TemplatePart(Name = ElementSeconds, Type = typeof(NumericUpDown))]
  [TemplatePart(Name = ElementMilliseconds, Type = typeof(NumericUpDown))]
  [TemplatePart(Name = ElementDays, Type = typeof(TextBox))]
  public class TimeSpanChooser : Control, INotifyPropertyChanged
  {
    private const string ElementDays = "PART_Days";
    private const string ElementHours = "PART_Hours";
    private const string ElementMinutes = "PART_Minutes";
    private const string ElementSeconds = "PART_Seconds";
    private const string ElementMilliseconds = "PART_Milliseconds";

    private Control _days;
    private Control _hours;
    private Control _minutes;
    private Control _seconds;
    private Control _milliseconds;

    public static readonly RoutedEvent ValueChangedEvent
        = EventManager.RegisterRoutedEvent("ValueChanged",
                                           RoutingStrategy.Bubble,
                                           typeof(RoutedPropertyChangedEventHandler<double?>),
                                           typeof(TimeSpanChooser));

    public static readonly DependencyProperty ValueProperty
      = DependencyProperty.Register("Value",
                                    typeof(TimeSpan),
                                    typeof(TimeSpanChooser),
                                    new FrameworkPropertyMetadata(default(TimeSpan),
                                                                  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public static readonly DependencyProperty EditModeProperty
      = DependencyProperty.Register("EditMode",
                                    typeof(Mode),
                                    typeof(TimeSpanChooser)
                                    );

    public event PropertyChangedEventHandler PropertyChanged;

    static TimeSpanChooser()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanChooser), new FrameworkPropertyMetadata(typeof(TimeSpanChooser)));
    }

    public TimeSpanChooser()
    {
      EditMode = Mode.None;
    }

    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
      if ((bool)e.NewValue) // has focus now
      {
        EditMode = Mode.Components;
      }
      else
      {
        EditMode = Mode.None;
      }      
    }

    protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
    {
      base.OnTemplateChanged(oldTemplate, newTemplate);
      if (newTemplate.Resources == null)
      {
        return;
      }
      if (!newTemplate.Resources.Contains("EditComponentsMode"))
      {
        return;
      }

      Keyboard.Focus(this);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      //base.OnPreviewKeyDown(e);

      //if (EditMode != Mode.Components)
      //{
      //  return;
      //}

      //  var keyBindings = new Map<Key, Action>(Do.Nothing)
      //{
      //  [Key.Left] = SetFocusToPreviousComponent,
      //  [Key.Right] = SetFocusToNextComponent,
      //};

      //keyBindings[e.Key]();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);
      if (EditMode != Mode.Components)
      {
        return;
      }

      var keyBindings = new Map<Key, Action>(Do.Nothing)
      {
        [Key.Left] = SetFocusToPreviousComponent,
        [Key.Right] = SetFocusToNextComponent,
      };

      keyBindings[e.Key]();
    }

    private void SetFocusToNextComponent()
    {
      Keyboard.Focus(_minutes);
    }

    private void SetFocusToPreviousComponent()
    {
      Keyboard.Focus(_seconds);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (EditMode != Mode.Components)
      {
        _days = null;
        _hours = null;
        _minutes = null;
        _seconds = null;
        _milliseconds = null;
      }
      else
      {
        _days = GetTemplateChild(ElementDays) as Control;
        _hours = GetTemplateChild(ElementHours) as Control;
        _minutes = GetTemplateChild(ElementMinutes) as Control;
        _seconds = GetTemplateChild(ElementSeconds) as Control;
        _milliseconds = GetTemplateChild(ElementMilliseconds) as Control;
      }
    }    

    public Mode EditMode
    {
      get { return (Mode)GetValue(EditModeProperty); }
      set { SetValue(EditModeProperty, value); }
    }

    public TimeSpan Value
    {
      get { return (TimeSpan)GetValue(ValueProperty); }
      set
      {
        SetValue(ValueProperty, value);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Days)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hours)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Minutes)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Seconds)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Milliseconds)));
      }
    }

    public string ValueAsText
    {
      get { return (string)new TimeSpanToText().Convert(Value, typeof(string), null, CultureInfo.InvariantCulture); }
      set { Value = (TimeSpan)new TimeSpanToText().ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture); }
    }

    public int Days
    {
      get { return Value.Days; }
      set { Value = new TimeSpan(value, Value.Hours, Value.Minutes, Value.Seconds, Value.Milliseconds); }
    }

    public int Hours
    {
      get { return Value.Hours; }
      set { Value = new TimeSpan(Value.Days, value, Value.Minutes, Value.Seconds, Value.Milliseconds); }
    }

    public int Minutes
    {
      get { return Value.Minutes; }
      set { Value = new TimeSpan(Value.Days, Value.Hours, value, Value.Seconds, Value.Milliseconds); }
    }

    public int Seconds
    {
      get { return Value.Seconds; }
      set { Value = new TimeSpan(Value.Days, Value.Hours, Value.Minutes, value, Value.Milliseconds); }
    }

    public int Milliseconds
    {
      get { return Value.Milliseconds; }
      set { Value = new TimeSpan(Value.Days, Value.Hours, Value.Minutes, Value.Seconds, value); }
    }
  }

  public enum Mode
  {
    /// <summary>
    /// Representation (non-edit mode).
    /// </summary>
    None,
    /// <summary>
    /// Enter the timespan as a text. The value is parsed into the components
    /// d(ays), h(ours), m(inutes) s(econds) and ms (milliseconds).
    /// </summary>
    Text,
    /// <summary>
    /// Modify the timespan components using the arrow keys.
    /// </summary>
    Components
  }
}