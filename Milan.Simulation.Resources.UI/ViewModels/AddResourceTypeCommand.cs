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
  public class AddResourceTypeCommand : PropertyChangedBase
  {
    private readonly Action<IResourceType> _addResource;
    private readonly IModel _model;

    public AddResourceTypeCommand(IModel model, Action<IResourceType> addResource, IEnumerable<IResourceTypeAmount> exceptTheseResourceTypeAmounts)
    {
      _model = model;
      _addResource = addResource;

      Refresh(exceptTheseResourceTypeAmounts);
    }

    public string DisplayText { get; private set; }
    public IEnumerable<ParameterValue> ValuesForFirstParameter { get; private set; }

    public void Refresh(IEnumerable<IResourceTypeAmount> exceptTheseResources)
    {
      DisplayText = "Add";

      if (_model == null)
      {
        // Inactive state
        return;
      }
      
      var resourceTypes = from resourceType in _model.Entities.OfType<IResourceType>()
                          orderby resourceType.Name
                          where exceptTheseResources.All(ptsr => ptsr.ResourceType != resourceType)
                          select resourceType;
      var entityValues = resourceTypes.Select(rta => new ParameterValue(rta, o => _addResource((IResourceType) o), _ => new ParameterValue[0]))
                                      .ToArray();


      ValuesForFirstParameter = entityValues;
      NotifyOfPropertyChange(() => ValuesForFirstParameter);
    }
  }
}