#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.WPF.ViewModels;

namespace Emporer.WPF.Factories
{
  public interface IEditViewModelFactory
  {
    bool CanHandle(object model);
    IEditViewModel CreateEditViewModel(object model);
  }
}