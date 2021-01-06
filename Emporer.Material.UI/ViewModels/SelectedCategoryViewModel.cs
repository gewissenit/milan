#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;

namespace Emporer.Material.UI.ViewModels
{
  public class SelectedCategoryViewModel : PropertyChangedBase
  {
    private readonly IEnumerable<ICategory> _categories;
    private readonly ObservableCollection<SelectedCategoryViewModel> _childCategories = new ObservableCollection<SelectedCategoryViewModel>();
    private readonly ICategory _model;
    private readonly ICategorizable _parentCategorizable;
    private bool _isIndirectlySelected;

    public SelectedCategoryViewModel(ICategory model, IEnumerable<ICategory> categories, ICategorizable parentCategorizable)
    {
      _model = model;
      _categories = categories;

      _parentCategorizable = parentCategorizable;
      _parentCategorizable.PropertyChanged += (s, e) => UpdateSelectionIndicators();
      _categories.Where(c => c.ParentCategory == model)
                  .ForEach(AddChild);

      UpdateSelectionIndicators();
    }

    public ICategory Category
    {
      get { return _model; }
    }

    public IEnumerable<SelectedCategoryViewModel> ChildCategories
    {
      get { return _childCategories; }
    }

    public bool IsSelected
    {
      get { return _parentCategorizable.Categories.Contains(_model); }
      set
      {
        var isSelected = _parentCategorizable.Categories.Contains(_model);

        if (isSelected == value)
        {
          return;
        }

        if (value)
        {
          _parentCategorizable.Add(_model);
        }
        else
        {
          _parentCategorizable.Remove(_model);
        }
        NotifyOfPropertyChange(() => IsSelected);
      }
    }

    public bool IsIndirectlySelected
    {
      get { return _isIndirectlySelected; }
      internal set
      {
        _isIndirectlySelected = value;
        NotifyOfPropertyChange(() => IsIndirectlySelected);
      }
    }

    private void AddChild(ICategory childCategory)
    {
      var childCategoryVm = new SelectedCategoryViewModel(childCategory, _categories, _parentCategorizable);
      _childCategories.Add(childCategoryVm);
    }

    private void UpdateSelectionIndicators()
    {
      IsSelected = _parentCategorizable.Categories.Contains(Category);
      IsIndirectlySelected = ChildCategories.Any(cc => cc.IsSelected || cc.IsIndirectlySelected);
    }
  }
}