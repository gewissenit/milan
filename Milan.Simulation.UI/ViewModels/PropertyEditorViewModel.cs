#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF;
using Emporer.WPF.Factories;
using System.Diagnostics;

namespace Milan.Simulation.UI.ViewModels
{
  [Export(typeof(IPropertyEditorViewModel))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class PropertyEditorViewModel : PropertyChangedBase, IPropertyEditorViewModel
  {
    private readonly IEnumerable<IEditViewModelFactory> _editViewModelFactories;
    private IHaveDisplayName _editItem;
    private ISelection _selection;

    [ImportingConstructor]
    public PropertyEditorViewModel([ImportMany] IEnumerable<IEditViewModelFactory> editViewModelFactories)
    {
      _editViewModelFactories = editViewModelFactories;
    }

    public IHaveDisplayName EditItem
    {
      get { return _editItem; }
      private set
      {
        _editItem = value;
        NotifyOfPropertyChange(() => EditItem);
      }
    }

    public ISelection Selection
    {
      get { return _selection; }
      set
      {
        _selection = value;
        _selection.Subscribe<object>(this, SetEditItem);
        SetEditItem(_selection.Current);
      }
    }

    public void SetEditItem(object item)
    {
      if (item == null)
      {
        EditItem = null;
        return;
      }
      //single item?
      item = ToSingleObjectOrEnumerable(item);
      EditItem = _editViewModelFactories.Single(w => w.CanHandle(item))
                                        .CreateEditViewModel(item);
    }

    private object ToSingleObjectOrEnumerable(object item)
    {
      var enumerable = item as IEnumerable;
      if (enumerable == null)
      {
        return item; // not a list, just use it whatever it is
      }
      var items = enumerable.Cast<object>().ToArray();
      if (items.Length==1)
      {
        return items[0];
      }
      return item;
    }
  }
}