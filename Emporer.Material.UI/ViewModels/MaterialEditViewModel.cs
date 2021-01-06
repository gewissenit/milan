#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;

namespace Emporer.Material.UI.ViewModels
{
  public class MaterialEditViewModel : Conductor<IHaveDisplayName>.Collection.OneActive, IEditViewModel
  {
    private readonly IMaterial _model;

    public MaterialEditViewModel(IMaterial model,
                                 IEnumerable<Screen> sections)
    {
      _model = model;
      Items.AddRange(sections);

      ((IActivate) this).Activate();
      ActivateItem(Items.First());
    }
    
    public string Description
    {
      get { return _model.Description; }
      set
      {
        _model.Description = value;
        NotifyOfPropertyChange(() => Description);
      }
    }

    public object Model => _model;

    public sealed override void ActivateItem(IHaveDisplayName item)
    {
      base.ActivateItem(item);
    }
  }
}