#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Emporer.Material.UI.ViewModels
{
  public class MaterialViewModel : PropertyChangedBase
  {
    public MaterialViewModel(IMaterial model)
    {
      Model = model;
    }

    public IMaterial Model { get; private set; }
  }
}