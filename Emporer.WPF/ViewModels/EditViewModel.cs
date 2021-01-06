#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Emporer.WPF.ViewModels
{
  public abstract class EditViewModel : PropertyChangedBase, IEditViewModel
  {
    private readonly string _displayName;

    public EditViewModel(object model, string name)
    {
      Model = model;
      _displayName = name;
    }

    public object Model { get; private set; }

    public string DisplayName
    {
      get { return _displayName; }
      set { }
    }
  }
}