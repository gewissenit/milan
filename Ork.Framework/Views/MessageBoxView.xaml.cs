using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ork.Framework.ViewModels;

namespace Ork.Framework.Views
{
  /// <summary>
  ///   Interaction logic for MessageBoxView.xaml
  /// </summary>
  public partial class MessageBoxView : UserControl
  {
    public MessageBoxView()
    {
      InitializeComponent();
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      Console.WriteLine("Message Box received focus.");
    }
  }
}