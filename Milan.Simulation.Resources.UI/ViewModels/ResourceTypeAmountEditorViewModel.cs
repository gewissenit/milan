#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ResourceTypeAmountEditorViewModel : PropertyChangedBase
  {
    private readonly IResourceTypeAmount _model;

    public ResourceTypeAmountEditorViewModel(IResourceTypeAmount resourceTypeAmount)
    {
      _model = resourceTypeAmount;
      _model.PropertyChanged += ReactToModelChange;
    }

    public string Description
    {
      get { return string.Format("{0}", ResourceType.Name); }
    }

    public IResourceTypeAmount Model
    {
      get { return _model; }
    }

    public int Amount
    {
      get { return _model.Amount; }
      set { _model.Amount = value; }
    }

    public IResourceType ResourceType
    {
      get { return _model.ResourceType; }
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Amount")
      {
        NotifyOfPropertyChange(() => Amount);
      }
    }
  }
}