#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class InfluenceRateEditorViewModel : PropertyChangedBase
  {
    private readonly IInfluenceRate _model;

    public InfluenceRateEditorViewModel(IInfluenceRate model)
    {
      _model = model;
      _model.PropertyChanged += ReactToModelChange;
    }
    
    public IInfluenceRate Model
    {
      get { return _model; }
    }

    public IInfluence Influence
    {
      get { return _model.Influence; }
    }

    public double Value
    {
      get { return _model.Value; }
      set { _model.Value = value; }
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Value":
          NotifyOfPropertyChange(() => Value);
          return;
      }
    }
  } 
}