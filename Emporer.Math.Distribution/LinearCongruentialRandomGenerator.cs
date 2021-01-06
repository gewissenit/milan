#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Uniform pseudo random generator producing a stream of uniform distributed [0,1] double values.
  ///   Implements the IUniformRandomGenerator interface.
  ///   This pseudo random generator is based on the linear congruential method and
  ///   implements a prime modulus multiplicative LCG (PMMLCG),
  ///   see Law and Kelton, p 227.
  /// </summary>
  public class LinearCongruentialRandomGenerator : Object, IUniformRandomGenerator
  {
    // modulus
    private const long DefaultSeed = 1973272912;

    #region Constructor(s)

    /// <summary>
    ///   Initializes a new instance of the <see cref="LinearCongruentialRandomGenerator" /> class,
    ///   a linear congruential random Generator with default values:
    ///   a = 16807
    ///   seed = 1973272912
    ///   m = 214783647
    /// </summary>
    public LinearCongruentialRandomGenerator()
    {
      Initialize();
    }

    #endregion

    #region Seed

    /// <summary>
    ///   Gets or sets the actual seed for the pseudo random number generator.
    /// </summary>
    /// <value>The actual seed for the random number stream.</value>
    private long _Seed = DefaultSeed;

    public long Seed
    {
      get { return _Seed; }
      set
      {
        if ((value > 0) &&
            (value <= _m))
        {
          _Seed = value;
          _z = value;
        }
        else
        {
          throw new InvalidOperationException("Wrong value for the seed parameter! " + "\n\r" + GetType()
                                                                                                  .Name + " :  Method: setSeed(seed)" + "\n\r" +
                                              "You tried to set the seed of a PMMLCG to a wrong seed value ." + "\n\r" +
                                              "The seed value should be > 0 and <= 2147483647." + "\n\r" + "New value ignored !" + "\n\r" +
                                              "Be sure to set a proper value for the seed parameter.");
        }
      }
    }

    #endregion

    #region Initialize

    /// <summary>
    ///   Initializes this instance.
    /// </summary>
    private void Initialize()
    {
      _z = Seed;
      _m = 2147483647;
    }

    #endregion

    #region NextDouble

    /// <summary>
    ///   Returns the next pseudo random uniform [0,1] distributed double value from the stream
    ///   produced by the underlying pseudo random number generator.
    /// </summary>
    /// <returns>The next pseudo random uniform [0,1] distributed double value.</returns>
    public double NextDouble()
    {
      // Function in accordance with Scharge's portable FORTRAN generator
      // see Law and Kelton, p 428ff
      const int B15 = 32768;
      const int B16 = 65536;
      const int MULT1 = 24112;
      const int MULT2 = 26143;
      long XHI15;
      long LOWPRD;
      long LOW15;
      long FHI31;
      long OVFLOW;
      long ZI;
      ZI = _z;
      XHI15 = ZI / B16;
      LOWPRD = (ZI - XHI15 * B16) * MULT1;
      LOW15 = LOWPRD / B16;
      FHI31 = XHI15 * MULT1 + LOW15;
      OVFLOW = FHI31 / B15;
      ZI = (((LOWPRD - LOW15 * B16) - _m) + (FHI31 - OVFLOW * B15) * B16) + OVFLOW;
      if (ZI < 0)
      {
        ZI = ZI + _m;
      }
      XHI15 = ZI / B16;
      LOWPRD = (ZI - XHI15 * B16) * MULT2;
      LOW15 = LOWPRD / B16;
      FHI31 = XHI15 * MULT2 + LOW15;
      OVFLOW = FHI31 / B15;
      ZI = (((LOWPRD - LOW15 * B16) - _m) + (FHI31 - OVFLOW * B15) * B16) + OVFLOW;
      if (ZI < 0)
      {
        ZI = ZI + _m;
      }
      _z = ZI;
      return (2 * (ZI / 256) + 1) / 16777216.0;
    }

    #endregion

    #region Reset

    /// <summary>
    ///   Sets the pseudo random stream to the start value.
    /// </summary>
    public void Reset()
    {
      Seed = DefaultSeed;
    }

    #endregion

    private int _m;
    // startig value
    private long _z;

    internal void NextBytes(byte[] buffer)
    {
      var bufLen = buffer.Length;

      if (buffer == null)
      {
        throw new ArgumentNullException();
      }

      for (var idx = 0; idx < bufLen; ++idx)
      {
        buffer[idx] = (byte) Next(256);
      }
    }

    internal int Next(int maxValue)
    {
      if (maxValue < 1)
      {
        if (maxValue < 0)
        {
          throw new ArgumentOutOfRangeException();
        }

        return 0;
      }
      var result = (int) (NextDouble() * ((long) maxValue + 1));
      if (result <= maxValue)
      {
        return result;
      }
      return maxValue;
    }
  }
}