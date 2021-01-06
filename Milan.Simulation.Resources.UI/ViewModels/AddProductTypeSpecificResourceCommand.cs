#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class AddProductTypeSpecificResourceCommand : PropertyChangedBase
  {
    private readonly Action<IProductType, IResourcePool, IResourceType> _addResource;
    private readonly IModel _model;
    private IProductType _chosenProductType;
    private IResourcePool _chosenResourcePool;
    private IResourceType _chosenResourceType;

    public AddProductTypeSpecificResourceCommand(IModel model, Action<IProductType, IResourcePool, IResourceType> addResource, IEnumerable<IProductTypeSpecificResource> exceptTheseResourceTypeAmounts)
    {
      _model = model;
      _addResource = addResource;

      Refresh(exceptTheseResourceTypeAmounts);
    }

    public string DisplayText { get; private set; }
    public IEnumerable<ParameterValue> ValuesForFirstParameter { get; private set; }

    private void ChooseResourceType(object obj)
    {
      _chosenResourceType = (IResourceType) obj;
      _addResource(_chosenProductType, _chosenResourcePool, _chosenResourceType);
    }

    private void ChooseProductType(object obj)
    {
      _chosenProductType = (IProductType) obj;
    }

    private void ChooseResourcePool(object obj)
    {
      _chosenResourcePool = (IResourcePool) obj;
    }

    public void Refresh(IEnumerable<IProductTypeSpecificResource> exceptTheseResources)
    {
      DisplayText = "Add";

      if (_model == null)
      {
        // Inactive state
        return;
      }
      var resourcePools = _model.Entities.OfType<IResourcePool>()
                               .ToArray();
      var productTypes = from productType in _model.Entities.OfType<IProductType>()
                         from resourcePool in resourcePools
                         from resourceTypeAmount in resourcePool.Resources
                         where !exceptTheseResources.Any(ptsr => ptsr.ResourceType == resourceTypeAmount.ResourceType && ptsr.ResourcePool == resourcePool && ptsr.ProductType == productType)
                         select productType;

      var entityValues = productTypes.Distinct()
                                     .OrderBy(x => x.Name)
                                     .Select(v => new ParameterValue(v,
                                                                     ChooseProductType,
                                                                     w =>
                                                                     {
                                                                       var rps = from resourcePool in resourcePools.OrderBy(rp => rp.Name)
                                                                                           from resourceTypeAmount in resourcePool.Resources
                                                                                           where !exceptTheseResources.Any(ptsr => ptsr.ResourceType == resourceTypeAmount.ResourceType && ptsr.ResourcePool == resourcePool && ptsr.ProductType == w)
                                                                                           select resourcePool;

                                                                       return rps.Distinct()
                                                                                           .Select(x => new ParameterValue(x,
                                                                                                                           ChooseResourcePool,
                                                                                                                           y =>
                                                                                                                           {
                                                                                                                             var resourceTypes = from resourceType in x.Resources.Select(rta => rta.ResourceType)
                                                                                                                                                 orderby resourceType.Name
                                                                                                                                                 where !exceptTheseResources.Any(ptsr => ptsr.ResourceType == resourceType && ptsr.ResourcePool == y && ptsr.ProductType == w)
                                                                                                                                                 select resourceType;
                                                                                                                             return resourceTypes.Select(rta => new ParameterValue(rta, ChooseResourceType, _ => new ParameterValue[0]));
                                                                                                                           }))
                                                                                           .ToArray();
                                                                     }))
                                     .ToArray();


      ValuesForFirstParameter = entityValues;
      NotifyOfPropertyChange(() => ValuesForFirstParameter);
    }
  }
}