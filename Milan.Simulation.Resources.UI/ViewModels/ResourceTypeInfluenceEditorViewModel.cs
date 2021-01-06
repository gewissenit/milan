using Caliburn.Micro;
using System.ComponentModel;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ResourceTypeInfluenceEditorViewModel : PropertyChangedBase
  {
    private readonly IResourceTypeInfluence _model;

    public ResourceTypeInfluenceEditorViewModel(IResourceTypeInfluence model)
    {
      _model = model;
      _model.PropertyChanged += ReactToModelChange;
    }

    public IResourceTypeInfluence Model
    {
      get { return _model; }
    }

    public IInfluence Influence
    {
      get { return _model.Influence; }
    }

    public double IncreaseFactor
    {
      get { return _model.IncreaseFactor; }
      set { _model.IncreaseFactor = value; }
    }

    public double RecoveryRate
    {
      get { return _model.RecoveryRate; }
      set { _model.RecoveryRate = value; }
    }

    public double InitialValue
    {
      get { return _model.InitialValue; }
      set { _model.InitialValue = value; }
    }

    public double LowerLimit
    {
      get { return _model.LowerLimit; }
      set { _model.LowerLimit = value; }
    }

    public double UpperLimit
    {
      get { return _model.UpperLimit; }
      set { _model.UpperLimit = value; }
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      // tunnel event
      NotifyOfPropertyChange(e.PropertyName);
    }
  }
}
