using System;

namespace GeWISSEN.Utils
{
  public static class EventExtensions
  {
    public static void Raise(this EventHandler action, object sender)
    {
      if (action == null)
      {
        return;
      }
      action(sender, new EventArgs());
    }

    public static void Raise<TEventArgs>(this EventHandler<TEventArgs> action, object sender, TEventArgs e)
    {
      if (action == null)
      {
        return;
      }
      action(sender, e);
    }

    public static void Raise(this Action action)
    {
      if (action == null)
      {
        return;
      }
      action();
    }

    public static void Raise<T>(this Action<T> action, T p)
    {
      if (action == null)
      {
        return;
      }
      action(p);
    }

    public static void Raise<T1, T2>(this Action<T1, T2> action, T1 p1, T2 p2)
    {
      if (action == null)
      {
        return;
      }
      action(p1, p2);
    }

    public static void Raise<T1, T2, T3>(this Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3)
    {
      if (action == null)
      {
        return;
      }
      action(p1, p2, p3);
    }
  }
}