#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Text;

namespace Emporer.Math.Distribution
{
  public class GammaDistribution : Distribution, IGammaDistribution, IRealDistribution
  {
    #region Consturctor(s)

    /// <summary>
    ///   Initializes a new instance of the <see cref="GammaDistribution" /> class, a stream of pseudo random numbers
    ///   following a gamma distribution with scale and shape parameter.
    ///   Note that shape and scale parameter has to be greater than 0.
    /// </summary>
    /// <param name="shape">The shape parameter for this distribution.</param>
    /// <param name="scale">The scale parameter for this distribution.</param>
    public GammaDistribution(double shape, double scale)
    {
      Shape = shape;
      Scale = scale;
      Minimum = 0;
    }

    public GammaDistribution()
    {
    }

    #endregion

    #region Scale

    private double _Scale;

    /// <summary>
    ///   Gets or sets the scale or beta parameter for the gamma distribution.
    /// </summary>
    /// <value>The scale parameter (beta) for the gamma distribution</value>
    public double Scale
    {
      get { return _Scale; }
      set
      {
        _Scale = value;
        IsValid = (Scale >= 0) && (Shape >= 0) && (Minimum >= 0);
      }
    }

    #endregion

    #region Shape

    private double _Shape;

    /// <summary>
    ///   Gets or sets the shape or alpha parameter for the Gamma distribution.
    /// </summary>
    /// <value>The shape parameter (alpha) for the Gamma distribution.</value>
    public double Shape
    {
      get { return _Shape; }
      set
      {
        _Shape = value;
        IsValid = (Scale >= 0) && (Shape >= 0) && (Minimum >= 0);
      }
    }

    #endregion

    #region Minimum

    /// <summary>
    ///   Getter and Setter fot the minimum parameter, where the distribution starts
    ///   on the variableValue-axis.
    /// </summary>
    private double _Minimum;

    public double Minimum
    {
      get { return _Minimum; }
      set
      {
        _Minimum = value;
        IsValid = (Scale >= 0) && (Shape >= 0) && (Minimum >= 0);
      }
    }

    #endregion

    #region Sample (override)

    /// <summary>
    ///   Returns the next floating point Sample from this gamma distribution.
    /// </summary>
    /// <returns>The next floating point Sample from this gamma distribution.</returns>
    double IRealDistribution.GetSample()
    {
      if (IsValid)
      {
        double r;
        if (Shape == 1.0)
        {
          do
          {
            r = RandomGenerator.NextDouble();
          } while (r == 0.0);
          if ((IsAntithetic))
          {
            r = -System.Math.Log(1 - r);
          }
          else
          {
            r = -System.Math.Log(r);
          }
        }
        else
        {
          bool condition;
          if (Shape < 1.0)
          {
            var e = System.Math.Exp(1.0);
            var b = (e + Shape) / e;
            condition = false;
            do
            {
              r = RandomGenerator.NextDouble();
              double m = RandomGenerator.NextDouble();
              if ((IsAntithetic))
              {
                r = 1 - r;
                m = 1 - m;
              }
              var p = b * r;
              if (p > 1.0)
              {
                var y = -System.Math.Log((b - p) / _Shape);
                if (m <= System.Math.Pow(y, (_Shape - 1)))
                {
                  r = y;
                  condition = true;
                }
              }
              else
              {
                var y = System.Math.Pow(p, (1 / _Shape));
                if (m <= System.Math.Exp(-y))
                {
                  r = y;
                  condition = true;
                }
              }
            } while (!(condition));
          }
          else
          {
            var b = Shape - System.Math.Log(4.0);
            condition = false;
            do
            {
              r = RandomGenerator.NextDouble();
              double m = RandomGenerator.NextDouble();
              if ((IsAntithetic))
              {
                r = 1 - r;
                m = 1 - m;
              }
              var y = Shape * System.Math.Exp(1 / (System.Math.Sqrt(2 * Shape - 1)) * System.Math.Log(r / (1 - r)));
              var z = System.Math.Pow(r, 2) * m;
              var w = b + Shape + (System.Math.Sqrt(2 * Shape - 1)) - y;
              if ((w + (1 + System.Math.Log(4.5)) - 4.5 * z) >= 0)
              {
                r = y;
                condition = true;
              }
              else
              {
                if (w >= System.Math.Log(z))
                {
                  r = y;
                  condition = true;
                }
              }
            } while (!(condition));
          }
        }
        r = Scale * r;
        return r + _Minimum;
      }
      throw new InvalidOperationException(Validate());
    }

    #endregion

    protected override StringBuilder ValidateCore(StringBuilder messages)
    {
      base.ValidateCore(messages);
      if ((Shape < 0))
      {
        messages.Append("Please check the Shape of the distribution. The Shape is not valid when it's value is lower than 0.");
      }
      if ((Scale < 0))
      {
        messages.Append("Please check the Scale of the distribution. The Scale is not valid when it's value is lower than 0.");
      }
      if ((Minimum < 0))
      {
        messages.Append("Please check the Minimum of the distribution. The Minimum is not valid when it's value is lower than 0.");
      }
      return messages;
    }
  }
}