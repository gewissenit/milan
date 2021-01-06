using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Ork.Framework.Properties;

namespace Ork.Framework
{
  public class MetroWindowManager : WindowManager
  {
    protected override Window EnsureWindow(object model, object view, bool isDialog)
    {
      MetroWindow window = null;
      Window inferOwnerOf;
      if (view is MetroWindow)
      {
        window = CreateCustomWindow(view, true);
        inferOwnerOf = InferOwnerOf(window);
        if (inferOwnerOf != null && isDialog)
        {
          window.Owner = inferOwnerOf;
        }
      }

      if (window == null)
      {
        window = CreateCustomWindow(view, false);
      }

      ConfigureWindow(window);
      window.SetValue(View.IsGeneratedProperty, true);
      inferOwnerOf = InferOwnerOf(window);
      if (inferOwnerOf != null)
      {
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Owner = inferOwnerOf;
      }

      return window;
    }

    public virtual void ConfigureWindow(MetroWindow window)
    {
      var state = GetWindowPrefferedWindowState();
      var location = GetWindowPrefferedWindowLocation();
      var size = GetWindowPrefferedWindowSize();

      window.WindowStartupLocation = WindowStartupLocation.Manual;
      window.Width = size.Width;
      window.Height = size.Height;
      window.Top = location.Y;
      window.Left = location.X;
      window.WindowState = state;
      window.Closing += SaveWindowProperties;
    }

    public virtual MetroWindow CreateCustomWindow(object view, bool windowIsView)
    {
      MetroWindow result;
      if (windowIsView)
      {
        result = view as MetroWindow;
      }
      else
      {
        result = new MetroWindow
                 {
                   Content = view
                 };
      }

      return result;
    }


    private Point GetWindowPrefferedWindowLocation()
    {
      return Settings.Default.AppWindowLocation;
    }

    private Size GetWindowPrefferedWindowSize()
    {
      return Settings.Default.AppWindowSize;
    }

    private WindowState GetWindowPrefferedWindowState()
    {
      return Settings.Default.AppWindowState;
    }

    private void SaveWindowProperties(object sender, CancelEventArgs e)
    {
      var window = (Window) sender;
      window.Closing -= SaveWindowProperties;

      Settings.Default.AppWindowState = window.WindowState;
      Settings.Default.AppWindowSize = new Size(window.ActualWidth, window.ActualHeight);
      Settings.Default.AppWindowLocation = new Point(window.Left, window.Top);
      Settings.Default.Save();
    }
  }
}