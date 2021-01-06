#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Caliburn.Micro;
using Emporer.Material;
using Emporer.WPF.ViewModels;

namespace Milan.UI.ViewModels
{
  internal class ContainedMaterialNavigationViewModel : Screen, IViewModel
  {
    private const string NoMaterialChosenMessage = @"Please choose the type of contained material";
    private readonly IContainedMaterial _model;

    public ContainedMaterialNavigationViewModel(IContainedMaterial model)
    {
      _model = model;
      UpdateDisplayName();
      _model.PropertyChanged += UpdateLocalProperties;
    }

    public object Model
    {
      get { return _model; }
    }

    private void UpdateLocalProperties(object sender, PropertyChangedEventArgs e)
    {
      UpdateDisplayName();
    }

    private void UpdateDisplayName()
    {
      DisplayName = _model.Material == null
                      ? NoMaterialChosenMessage
                      : string.Format("{1} {2} of {0}", _model.Material.Name, _model.Amount, _model.Material.DisplayUnit.Symbol);
    }
  }
}