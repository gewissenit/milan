#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Emporer.Math.Distribution
{
  public class MersenneTwisterRandomGenerator : IUniformRandomGenerator
  {
    #region ctor

    public MersenneTwisterRandomGenerator()
    {
      InitializeArray();
    }

    #endregion

    #region Private Fields (determining period)

    private const int N = 624;
    private const int M = 397;
    private const uint HIGH = 0x80000000;
    private const uint LOW = 0x7fffffff;

    private const long DefaultSeed = 5489;

    private static readonly uint[] A =
    {
      0x0, 0x9908b0df
    };

    #endregion

    #region State

    private readonly uint[] _State = new uint[N];

    private uint[] State
    {
      get { return _State; }
    }

    #endregion

    #region Index

    private uint Index { get; set; }

    #endregion

    private long _Seed = DefaultSeed;

    public long Seed
    {
      get { return _Seed; }
      set
      {
        if (value <= 0 ||
            value > uint.MaxValue)
        {
          throw new InvalidOperationException("Seed for Mersenne Twister must be > 0 and <= " + uint.MaxValue);
        }
        _Seed = value;
        InitializeArray();
      }
    }

    public double NextDouble()
    {
      return Generate() / ((double) uint.MaxValue + 1);
    }

    /// <summary>
    ///   Sets the pseudo random stream to the start value;
    /// </summary>
    public void Reset()
    {
      InitializeArray();
    }

    private uint Generate()
    {
      uint random;

      if (Index >= N)
      {
        for (short i = 0; i < N - M; ++i)
        {
          random = (State[i] & HIGH) | (State[i + 1] & LOW);
          State[i] = State[i + M] ^ (random >> 1) ^ A[random & 0x1];
        }

        for (short i = N - M; i < N - 1; ++i)
        {
          random = (State[i] & HIGH) | (State[i + 1] & LOW);
          State[i] = State[i + (M - N)] ^ (random >> 1) ^ A[random & 0x1];
        }

        random = (State[N - 1] & HIGH) | (State[0] & LOW);
        State[N - 1] = State[M - 1] ^ (random >> 1) ^ A[random & 0x1];

        Index = 0;
      }

      random = State[Index++];
      random ^= random >> 11;
      random ^= random << 7 & 0x9d2c5680;
      random ^= random << 15 & 0xefc60000;
      random ^= random >> 18;

      return random;
    }

    private void InitializeArray()
    {
      // setting initial seeds
      // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
      State[0] = (uint) Seed & 0xffffffff;
      for (Index = 1; Index < N; ++Index)
      {
        State[Index] = (1812433253 * (State[Index - 1] ^ (State[Index - 1] >> 30)) + Index) & 0xffffffff;
      }
    }

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