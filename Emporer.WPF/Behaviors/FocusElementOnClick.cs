using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Emporer.WPF.Behaviors
{
  public class FocusElementOnClick : Behavior<ButtonBase>
  {
    private ButtonBase _button;

    public static readonly DependencyProperty FocusElementProperty =
        DependencyProperty.Register("FocusElement", typeof(Control), typeof(FocusElementOnClick), new UIPropertyMetadata());

    public Control FocusElement
    {
      get { return (Control)GetValue(FocusElementProperty); }
      set { SetValue(FocusElementProperty, value); }
    }

    protected override void OnAttached()
    {
      _button = AssociatedObject;

      _button.Click += SetFocusToBoundElement;
    }

    protected override void OnDetaching()
    {
      _button.Click -= SetFocusToBoundElement;
    }

    private void SetFocusToBoundElement(object sender, RoutedEventArgs e)
    {
      Keyboard.Focus(FocusElement);
    }
  }
}
