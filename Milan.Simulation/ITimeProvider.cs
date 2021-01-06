#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation
{
  public interface ITimeProvider
  {
    double CurrentTime { get; }
  }
}