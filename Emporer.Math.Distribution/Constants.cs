#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Contains constants of the Distribution bundle.
  ///   Those are implemented in the Distribution.Internal bundle.
  /// </summary>
  public static class Constants
  {
    // error constants

    public const string ErrorDescSample = "Invalid Sample returned!";

    public const string ErrorReasonUndersizedList =
      "You tried to get a Sample from a RealDistList distribution, but the list of Sample is not big enough.";

    public const string ErrorPrevUndersizedList = "Extend the list of Samples or switch the periodic flag on to start with the first entry again.";
  }
}