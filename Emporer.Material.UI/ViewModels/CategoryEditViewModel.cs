#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.ViewModels
{
  public class CategoryEditViewModel : EditViewModel
  {
    private readonly ICategory _model;

    public CategoryEditViewModel(ICategory model, IEnumerable<ICategory> categories)
      : base(model, "Category")
    {
      _model = model;
      Categories = categories.Except(new[]
                                     {
                                       model
                                     })
                             .ToArray();
    }

    public IEnumerable<ICategory> Categories { get; private set; }

    public ICategory ParentCategory
    {
      get { return _model.ParentCategory; }
      set
      {
        if (_model.ParentCategory == value)
        {
          return;
        }
        _model.ParentCategory = value;
        NotifyOfPropertyChange(() => ParentCategory);
      }
    }

    public string Description
    {
      get { return _model.Description; }
      set
      {
        if (_model.Description == value)
        {
          return;
        }
        _model.Description = value;
        NotifyOfPropertyChange(() => Description);
      }
    }
  }
}