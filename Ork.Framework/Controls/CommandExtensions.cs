#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows;
using System.Windows.Input;

namespace Ork.Framework.Controls
{
  public class CommandExtensions : DependencyObject
  {
    public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command",
                                                                                                    typeof (ICommand),
                                                                                                    typeof (CommandExtensions),
                                                                                                    new UIPropertyMetadata(null));

    public static ICommand GetCommand(DependencyObject obj)
    {
      return (ICommand) obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(CommandProperty, value);
    }

    // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
  }
}