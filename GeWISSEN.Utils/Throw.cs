using System;

namespace GeWISSEN.Utils
{
  /// <summary>
  /// Throws exceptions.
  /// </summary>
  public static class Throw
  {
    /// <summary>
    /// When the given subject is null.
    /// </summary>
    /// <param name="subject">The subject to test.</param>
    /// <param name="name">An optional parameter name.</param>
    public static void IfNull(object subject, string name="")
    {
      if (subject!=null)
      {
        return;
      }
      throw new ArgumentNullException(name);
    }
  }
}
