#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Interface for uniform pseudo random number generators to be used by the distribution classes to compute
  ///   their samples. Random generators of this type must return uniform distributed double values.
  ///   Note that two constructors have to be implemented. One constructor without parameters to construct
  ///   a UniformRandomGenerator and another constructor with a parameter of type integer to pass the initial
  ///   seed value to the pseudo random generation algorithm.
  /// </summary>
  public interface IUniformRandomGenerator
  {
    long Seed { get; set; }

    /// <summary>
    ///   Returns the next uniform distributed [0,1] double pseudo random number from the random generator's stream.
    /// </summary>
    /// <returns> The next uniformly distributed [0,1] double value</returns>
    double NextDouble();

    /// <summary>
    ///   Sets the pseudo random stream to the start value;
    /// </summary>
    void Reset();
  }
}