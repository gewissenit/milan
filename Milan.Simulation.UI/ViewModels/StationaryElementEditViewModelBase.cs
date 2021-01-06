using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  /// <summary>
  ///   TODO: will replace StationaryElementEditViewModel soon
  /// </summary>
  public class StationaryElementEditViewModelBase<TModel> : Conductor<IHaveDisplayName>.Collection.OneActive, IEditViewModel
    where TModel : class, IEntity
  {
    private readonly TModel _model;

    protected StationaryElementEditViewModelBase(TModel model, IEnumerable<Screen> sections)
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
        if (_model.Description == value)
        {
          return;
        }
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