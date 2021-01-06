#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation
{
  public class DurationRelatedValue
  {
    public DurationRelatedValue(double value, double duration = 0)
    {
      Duration = duration;
      Value = value;
    }
    
    public double Duration { get; private set; }
    public double Value { get; private set; }
  }

  public class ProductTypeRelatedValue: DurationRelatedValue
  {
    public ProductTypeRelatedValue(double value, IProductType productType, double duration = 0)
      : base(value, duration)
    {
      ProductType = productType;
    }
    
    public IProductType ProductType { get; private set; }
  }
}