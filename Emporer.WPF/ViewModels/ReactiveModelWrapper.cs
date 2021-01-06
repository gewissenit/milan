#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using ReactiveUI;

namespace Emporer.WPF.ViewModels
{
  public abstract class ReactiveModelWrapper<TModel> : ReactiveObject, IViewModel
    where TModel : class
  {
    private readonly TModel _model;

    protected ReactiveModelWrapper(TModel model)
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }
      _model = model;
    }

    public TModel Model
    {
      get { return _model; }
    }

    object IViewModel.Model
    {
      get { return _model; }
    }
  }
}