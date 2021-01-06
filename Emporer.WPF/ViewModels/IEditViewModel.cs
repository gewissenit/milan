#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Caliburn.Micro;

namespace Emporer.WPF.ViewModels
{
  public interface IEditViewModel : IHaveDisplayName
  {
    object Model { get; }
  }
}