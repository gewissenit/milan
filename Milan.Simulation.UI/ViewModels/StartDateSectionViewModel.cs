#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Caliburn.Micro;

namespace Milan.Simulation.UI.ViewModels
{
  public class StartDateSectionViewModel : Screen
  {
    private readonly IModel _model;

    public StartDateSectionViewModel(IModel model)
    {
      DisplayName = "start date";
      _model = model;
    }

    public DateTime StartDate
    {
      get { return _model.StartDate; }
      set { _model.StartDate = value; }
    }
  }
}