#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class InfluenceViewModel : PropertyChangedBase
  {
    private readonly IInfluence _model;

    public InfluenceViewModel(IInfluence model)
    {
      _model = model;
      _model.PropertyChanged += ReactToModelChange;
    }


    public IInfluence Model
    {
      get { return _model; }
    }

    public string Name
    {
      get { return _model.Name; }      
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