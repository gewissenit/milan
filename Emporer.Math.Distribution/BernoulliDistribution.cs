#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  /// <summary>
  ///   Boolean Bernoulli distribution returning true values with the
  ///   given probability. Samples of this distribution can either be true or false
  ///   with a given probability
  ///   for value "true". The probabilitiy for "true" can only be set via the constructor.
  ///   It has to be a value between 0 and 1.
  ///   1.0 as values mean that always return "true".
  ///   0.0 as value mean that always return "false".
  /// </summary>
  public class BernoulliDistribution : Distribution, IBernoulliDistribution, IBoolDistribution
  {
    #region Probability

    private double _Probability;

    /// <summary>
    ///   Gets or sets the probability for true values as double
    ///   for maximum precision.
    /// </summary>
    /// <value>The probability of a true value being returned.</value>
    public double Probability
    {
      get { return _Probability; }
      set
      {
        _Probability = value;
        IsValid = CheckProbability(value);
      }
    }

    #endregion

    #region CheckProbability

    /// <summary>
    ///   Checks the probability to be a valid value.
    /// </summary>
    /// <param name="Probability">The probability.</param>
    /// <returns></returns>
    private static bool CheckProbability(double Probability)
    {
      return Probability <= 1.0 && Probability >= 0.0;
    }

    #endregion

    #region GetSample

    /// <summary>
    ///   Returns the next Bernoulli distributed Sample of the distribution.
    ///   The returned value will depend upon the seed of the underlying random generator and the
    ///   probability given for this distribution.
    ///   inherited from class reportable
    ///   direct mapping between probability [0,1] and Sample from RandomGenerator [0,1]
    ///   probability indicates level when to return "true".
    /// </summary>
    /// <returns>The next Bernoulli distributed random Sample.</returns>
    bool IBoolDistribution.GetSample()
    {
      if (IsValid)
      {
        if (IsAntithetic)
        {
          // inverted computation
          return ((1 - RandomGenerator.NextDouble()) < _Probability);
        }
        // normal computation
        return (RandomGenerator.NextDouble() < _Probability);
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if (!CheckProbability(Probability))
      {
        messages.Append(
                        "By a BernoulliDistribution a valid value for the probability for true have to be a value between 0.0 and 1.0. Please check this. ");
      }
      return messages;
    }
  }
}