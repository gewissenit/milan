#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.ViewModels
{
  internal class MaterialNavigationViewModel : Screen, IViewModel
  {
    private readonly IMaterial _model;

    public MaterialNavigationViewModel(IMaterial model)
    {
      _model = model;

      _model.PropertyChanged += UpdateRelatedLocalProperty;
    }

    public object Model
    {
      get { return _model; }
    }

    private void UpdateRelatedLocalProperty(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "Name":
          DisplayName = _model.Name;
          break;
      }
    }
  }
}