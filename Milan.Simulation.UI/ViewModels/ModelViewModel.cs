#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Milan.Simulation.UI.ViewModels
{
  public class ModelViewModel : PropertyChangedBase, IHaveDisplayName
  {
    public ModelViewModel(IModel model)
    {
      Model = model;
    }

    public IModel Model { get; private set; }

    public string DisplayName
    {
      get { return "Model"; }
      set { }
    }
  }
}