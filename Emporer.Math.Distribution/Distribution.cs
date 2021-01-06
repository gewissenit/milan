#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Base class for all pseudo random number distributions used in this package.
  ///   Defines a set of methods usefull for all kinds of random distributions that
  ///   can be based upon a stream of uniform distributed pseudo random numbers.
  ///   Prefabricated distributions implemented in this package can handle uniform,
  ///   normal (gaussian), bernoulli, poisson and heuristic distributions
  ///   with return values of the primitive data types double (floating point),
  ///   integer and boolean (true or false).
  ///   Inherit from this class if you want to implement new types of distributions
  ///   handing back values of other types than those listed above.
  ///   Basic idea is to use a pseudo random generator which produces a uniform
  ///   distributed stream of double numbers between 0 and 1
  ///   use inverse transformation to generate the desired distribution.
  ///   See also [Page91, p. 107]
  ///   Note that although this class implements all methods, it is set to be abstract,
  ///   since instantiating this class would not produce any meaningfull distribution
  ///   to be used by a client.
  /// </summary>
  public abstract class Distribution : IDistribution
  {
    #region RandomGenerator

    private IUniformRandomGenerator _RandomGenerator;

    /// <summary>
    ///   Gets or sets the random generator.
    ///   The underlying uniform pseudo random generator available to every distribution inheriting from this abstract class.
    ///   Valid generators have to implement the IUniformRandomGenerator interface.
    ///   By default class TLinearCongruentialRandomGenerator is used.
    ///   Note that changing the underlying random generator forces a Reset,
    ///   since a new generator might produce a completely different stream of pseudo random numbers
    ///   that won't enable us to reproduce the stream of numbers probably delivered by the previously used generator.
    ///   See Also:
    ///   <see cref="IUniformRandomGenerator" />
    ///   <see cref="LinearCongruentialRandomGenerator" />
    /// </summary>
    /// <value>The random generator.</value>
    public IUniformRandomGenerator RandomGenerator
    {
      get { return _RandomGenerator ?? (_RandomGenerator = new MersenneTwisterRandomGenerator()); }
      set
      {
        _RandomGenerator = value;
        Reset();
      }
    }

    #endregion

    #region IsAntithetic

    protected bool _IsAntithetic;

    /// <summary>
    ///   Gets or sets a value indicating whether this <see cref="Distribution" /> is antithetic.
    ///   If set to true, antithetic values are delivered. These depend upon the kind of distribution,
    ///   so this value here will probably be most useful to switch the algorithm in the implementation
    ///   of the abstract code Sample() method between "normal" and "antithetic" value generation.
    ///   This feature is not associated to the pseudo random generator since the algorithm
    ///   for calculating antithetic values might not require antithetic uniformly distributed values.
    ///   IsAntithetic random numbers are used to minimize the standard deviation of a
    ///   series of simulation runs. The Results of a run with normal random numbers
    ///   has to be standardized with the Results of a run using antithetic random
    ///   numbers, thus doubling the number of Samples needed, but also lowering the
    ///   standard deviation of the Results of that simulation. See [Page91, p.139].
    /// </summary>
    /// <value><c>true</c>antithetic mode on ; otherwise,  antithetic mode off<c>false</c>.</value>
    public virtual bool IsAntithetic
    {
      get { return _IsAntithetic; }
      set { _IsAntithetic = value; }
    }

    #endregion

    #region Seed

    /// <summary>
    ///   Gets or sets the seed of the underlying pseudorandom generator.
    ///   The seed value is passed on to the underlying RandomGenerator but since those generators are not supposed to keep
    ///   track of their initial
    ///   seed value it is stored here to make sure they are not lost.
    ///   The seed controls the starting value of the random generators and all
    ///   following generated pseudo random numbers.
    ///   Resetting the seed between two simulation runs will let you use identical
    ///   streams of random numbers. That will enable you to compare different
    ///   strategies within your model based on the same random number stream produced
    ///   by the random generator.
    /// </summary>
    /// <value>The seed.</value>
    public virtual long Seed
    {
      get { return RandomGenerator.Seed; }
      set { RandomGenerator.Seed = value; }
    }

    #endregion

    #region Reset

    /// <summary>
    ///   Resets the pseudo random generator's seed and the number of Samples given to zero.
    /// </summary>
    public virtual void Reset()
    {
      RandomGenerator.Reset();
    }

    /// <summary>
    ///   Resets the specified new seed.
    ///   Resets the pseudo random generator's seed to the value passed
    ///   and sets antithetic to false for this distribution. Acts the
    ///   same as a call of method Reset and a consecutive call to setSeed(integer).
    /// </summary>
    /// <param name="newSeed">The new seed to be used by underlying random number generator after Reset</param>
    public virtual void Reset(long newSeed)
    {
      RandomGenerator.Reset();
      Seed = newSeed;
    }

    #endregion

    #region Distribution Validation

    protected bool _IsValid = true;

    internal virtual bool IsValid
    {
      get { return _IsValid; }
      set { _IsValid = value; }
    }

    protected string Validate()
    {
      var messages = new StringBuilder();
      ValidateCore(messages);
      return messages.ToString();
    }

    protected virtual StringBuilder ValidateCore(StringBuilder messages)
    {
      messages.Append("Validation of the distribution: ");
      return messages;
    }

    #endregion
  }
}