using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Emporer.WPF.Behaviors
{
  /// <summary>
  ///   Captures and eats MouseWheel events so that a nested ListBox does not
  ///   prevent an outer scrollable control from scrolling.
  /// </summary>
  public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.PreviewMouseWheel += IgnoreEvent;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.PreviewMouseWheel -= IgnoreEvent;
      base.OnDetaching();
    }

    private void IgnoreEvent(object sender, MouseWheelEventArgs e)
    {
      e.Handled = true;

      var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
      e2.RoutedEvent = UIElement.MouseWheelEvent;

      AssociatedObject.RaiseEvent(e2);
    }
  }
}