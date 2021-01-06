namespace GeWISSEN.Utils
{
  /// <summary>
  /// When you need an empty delegate that does nothing, use this to explicitely state your intention to your fellow reviewers. ;)
  /// </summary>
  public static class Do
  {
    /// <summary>
    /// Does nothing.
    /// </summary>
    public static void Nothing() { }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public static void Nothing<T>(T _) { }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public static void Nothing<T1, T2>(T1 _, T2 __) { }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public static void Nothing<T1, T2, T3>(T1 _, T2 __, T3 ___) { }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public static void Nothing<T1, T2, T3, T4>(T1 _, T2 __, T3 ___, T4 ____) { }

    /// <summary>
    /// Does nothing.
    /// </summary>
    public static void Nothing<T1, T2, T3, T4, T5>(T1 _, T2 __, T3 ___, T4 ____, T5 _____) { }
  }
}