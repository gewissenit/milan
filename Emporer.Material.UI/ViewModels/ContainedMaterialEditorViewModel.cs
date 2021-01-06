#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.ViewModels
{
  public class ContainedMaterialEditorViewModel : EditViewModel
  {
    private readonly IContainedMaterial _model;
    private readonly IMaterial _parentMaterial;

    public ContainedMaterialEditorViewModel(IContainedMaterial model, IMaterial parentMaterial)
      : base(model, "Contained Material")
    {
      _model = model;
      _parentMaterial = parentMaterial;
    }


    public string MaterialDisplayUnit
    {
      get
      {
        if (_model.Material == null ||
            _model.Material.DisplayUnit == null)
        {
          return null;
        }
        return _model.Material.DisplayUnit.Symbol;
      }
    }

    public IMaterial Material
    {
      get { return _model.Material; }
      set
      {
        if (_model.Material == value)
        {
          return;
        }
        _model.Material = value;
        NotifyOfPropertyChange(() => Material);
        NotifyOfPropertyChange(() => MaterialDisplayUnit);
        NotifyOfPropertyChange(() => Conversion);
      }
    }

    public double Amount
    {
      get { return _model.Amount; }
      set
      {
        if (_model.Amount == value)
        {
          return;
        }
        _model.Amount = value;
        NotifyOfPropertyChange(() => Amount);
      }
    }

    public string Conversion
    {
      get
      {
        if (Amount <= 0)
        {
          return string.Empty;
        }
        else
        {
          return string.Format("1 {0} {1} = {2} {3} {4}\n1 {3} {4} = {5:0.###} {0} {1}",
                               _parentMaterial.DisplayUnit == null
                                 ? string.Empty
                                 : _parentMaterial.DisplayUnit.Symbol,
                               _parentMaterial.Name,
                               Amount,
                               Material.DisplayUnit.Symbol,
                               Material.Name,
                               1 / Amount);
        }
      }
    }

    
  }
}