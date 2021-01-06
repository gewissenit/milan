using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Emporer.WPF.Behaviors
{
  public class CommitOnEnter : Behavior<TextBox>
  {
    protected override void OnAttached()
    {
      AssociatedObject.KeyDown += RaiseChangeOnEnterKey;
    }

    private void RaiseChangeOnEnterKey(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Enter)
      {
        return;
      }
      var binding = BindingOperations.GetBindingExpression(AssociatedObject, TextBox.TextProperty);
      binding.UpdateSource();
    }


    protected override void OnDetaching()
    {
      AssociatedObject.KeyDown -= RaiseChangeOnEnterKey;
    }
  }
}