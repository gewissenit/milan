#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ResourceTypeViewModel : PropertyChangedBase
  {
    private readonly IResourceType _resourceType;

    public ResourceTypeViewModel(IResourceType resourceType)
    {
      _resourceType = resourceType;
      _resourceType.PropertyChanged += ReactToModelChange;
    }


    public IResourceType Model
    {
      get { return _resourceType; }
    }

    public string Name
    {
      get { return _resourceType.Name; }      
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          NotifyOfPropertyChange(() => Name);
          return;
      }
    }
  }
}