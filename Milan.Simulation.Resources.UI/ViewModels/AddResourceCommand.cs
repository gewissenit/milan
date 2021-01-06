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
  public class AddResourceCommand : PropertyChangedBase
  {
    private readonly Action<IResourcePool, IResourceType> _addResource;
    private readonly IModel _model;
    private IResourcePool _chosenResourcePool;
    private IResourceType _chosenResourceType;

    public AddResourceCommand(IModel model, Action<IResourcePool, IResourceType> addResource, IEnumerable<IResourcePoolResourceTypeAmount> exceptTheseResourceTypeAmounts)
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
      _addResource(_chosenResourcePool, _chosenResourceType);
    }

    private void ChooseResourcePool(object obj)
    {
      _chosenResourcePool = (IResourcePool) obj;
    }

    public void Refresh(IEnumerable<IResourcePoolResourceTypeAmount> exceptTheseResources)
    {
      DisplayText = "Add";

      if (_model == null)
      {
        // Inactive state
        return;
      }


      var resourcePools = from resourcePool in _model.Entities.OfType<IResourcePool>()
                                                     .OrderBy(cof => cof.Name)
                          from resourceTypeAmount in resourcePool.Resources
                          where !exceptTheseResources.Any(ptsr => ptsr.ResourceType == resourceTypeAmount.ResourceType && ptsr.ResourcePool == resourcePool)
                          select resourcePool;

      var entityValues = resourcePools.Distinct()
                                      .Select(x => new ParameterValue(x,
                                                                      ChooseResourcePool,
                                                                      y =>
                                                                      {
                                                                        var resourceTypes = from resourceTypeAmount in x.Resources
                                                                                            orderby resourceTypeAmount.ResourceType.Name
                                                                                            where !exceptTheseResources.Any(ptsr => ptsr.ResourceType == resourceTypeAmount.ResourceType && ptsr.ResourcePool == y)
                                                                                            select resourceTypeAmount.ResourceType;
                                                                        return resourceTypes.Select(rta => new ParameterValue(rta, ChooseResourceType, _ => new ParameterValue[0]));
                                                                      }))
                                      .ToArray();


      ValuesForFirstParameter = entityValues;
      NotifyOfPropertyChange(() => ValuesForFirstParameter);
    }
  }
}