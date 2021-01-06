#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation
{
  public class Product
  {
    private readonly long _id;
    private IStationaryElement _currentLocation;

    public Product(IModel model, IProductType productType, double timeStamp)
    {
      _id = model.GetIndexForDynamicEntity(typeof (Product));
      IntegratedProducts = new List<Product>();
      ProductType = productType;
      TimeStamp = timeStamp;
      ExperimentProperties = new List<object>();
    }
    
    public Product(IModel model, IProductType productType, double timeStamp, IEnumerable<Product> integratedProducts)
      : this(model, productType, timeStamp)
    {
      IntegratedProducts = integratedProducts.ToArray();
    }
    
    public long Id
    {
      get { return _id; }
    }

    public virtual IEnumerable<Product> IntegratedProducts { get; private set; }
    public double TimeStamp { get; set; }
    public IEntity EntryPoint { get; set; }
    public virtual IProductType ProductType { get; private set; }
    //todo: this is reporting stuff and should be tracked seperately
    public virtual ICollection<object> ExperimentProperties { get; private set; }

    public virtual IStationaryElement CurrentLocation
    {
      get { return _currentLocation; }
      set
      {
        var previousLocation = _currentLocation;
        _currentLocation = value;
        UpdatePath(_currentLocation, previousLocation);
      }
    }

    private void UpdatePath(IStationaryElement current, IStationaryElement previous)
    {
      var currentTime = current.CurrentExperiment.CurrentTime;

      var previousLocation = previous == null
                               ? "-"
                               : previous.Name;
      var currentLocation = current.Name;

      ExperimentProperties.Add(new LocationChangeInfo(previousLocation, currentTime, LocationChange.Departure));
      ExperimentProperties.Add(new LocationChangeInfo(currentLocation, currentTime, LocationChange.Arrival));
    }

    public override string ToString()
    {
      return string.Format("({0}({1})", ProductType.Name, Id);
    }
  }
}