#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Defines a set of methods useful for all kinds of random distributions that can be based upon a stream of uniform
  ///   distributed pseudo random numbers.
  /// </summary>
  public interface IDistribution
  {
    /// <summary>
    ///   Gets or sets the seed of the underlying pseudo-random generator.
    ///   The seed controls the starting value of the random generators and all
    ///   following generated pseudo random numbers.
    ///   Resetting the seed between two simulation runs will let you use identical
    ///   streams of random numbers. That will enable you to compare different
    ///   strategies within your model based on the same random number stream produced
    ///   by the random generator.
    /// </summary>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Seed")]
    [Category("Settings")]
    [Description(
      "The seed controls the starting value of the random generators and all following generated pseudo random numbers. Resetting the seed between two simulation runs will let you use identical streams of random numbers. That will enable you to compare different strategies within your model based on the same random number stream produced by the random generator."
      )]
    [EditorBrowsable(EditorBrowsableState.Always)]
    long Seed
    {
      get;
      set;
    }

    /// <summary>
    ///   Gets or sets a value indicating whether this distribution is antithetic.
    ///   If set to true, antithetic values are delivered.
    ///   This feature is not associated to the pseudo random generator since the algorithm
    ///   for calculating antithetic values might not require antithetic uniformly distributed values.
    ///   IsAntithetic random numbers are used to minimize the standard deviation of a
    ///   series of simulation runs. The Results of a run with normal random numbers
    ///   has to be standardized with the Results of a run using antithetic random
    ///   numbers, thus doubling the number of Samples needed, but also lowering the
    ///   standard deviation of the Results of that simulation. See [Page91, p.139].
    ///   Note that every change of this value need a reset of the pseudo random generator's seed and the number of Samples
    ///   given to zero
    /// </summary>
    [Browsable(true)]
    [ReadOnly(false)]
    [DisplayName("Is Antithetic")]
    [Category("Settings")]
    [Description(
      "If set to true, antithetic values are delivered. This feature is not associated to the pseudo random generator since the algorithm for calculating antithetic values might not require antithetic uniformly distributed values. IsAntithetic random numbers are used to minimize the standard deviation of a series of simulation runs. The Results of a run with normal random numbers has to be standardized with the Results of a run using antithetic random numbers, thus doubling the number of Samples needed, but also lowering the standard deviation of the Results of that simulation."
      )]
    [EditorBrowsable(EditorBrowsableState.Always)]
    bool IsAntithetic
    {
      get;
      set;
    }
    
    /// <summary>
    ///   Resets the pseudo random generator's seed and the number of Samples given to zero.
    /// </summary>
    void Reset();

    /// <summary>
    ///   Resets the specified new seed.
    ///   Resets the pseudo random generator's seed to the value passed
    ///   and sets antithetic to false for this distribution. Acts the
    ///   same as a call of method Reset and a consecutive call to setSeed(integer).
    /// </summary>
    void Reset(long newSeed);
  }
}