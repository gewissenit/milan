#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Statistics
{
  public struct NumberOfProductsAtPlace
  {
    public NumberOfProductsAtPlace(string place, string productTypeName, double number)
      : this()
    {
      Place = place;
      ProductTypeName = productTypeName;
      Number = number;
    }

    public NumberOfProductsAtPlace(string place, string productTypeName, IEnumerable<double> numbers)
      : this(place, productTypeName, numbers.Average())
    {
    }

    public string Place { get; private set; }
    public string ProductTypeName { get; private set; }
    public double Number { get; private set; }
  }
}