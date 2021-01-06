#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.Math.Distribution;
using NUnit.Framework;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class NormalDistributionFixture
  {
    [Test]
    public void What_This_Test_Demonstrates()
    {
      var sut = new LogNormalDistribution
                {
                  Mean = 5,
                  StandardDeviation = 2
                };


      var samples = new List<double>();
      for (var i = 0; i < 99999; i++)
      {
        samples.Add(((IRealDistribution) sut).GetSample());
      }
    }
  }
}