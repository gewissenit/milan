using System.ComponentModel;
using Caliburn.Micro;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ProductTypeSpecificResourceEditorViewModel : PropertyChangedBase, IEditViewModel
  {
    private readonly IProductTypeSpecificResource _model;
    public ProductTypeSpecificResourceEditorViewModel(IProductTypeSpecificResource resource)
    {
      _model = resource;
      _model.PropertyChanged += ReactToModelChange;
    }

    public object Model
    {
      get { return _model; }
    }

    public int Amount
    {
      get { return _model.Amount; }
      set { _model.Amount = value; }
    }

    public string Description
    {
      get { return string.Format("{0} - {1} ({2})", ProductType.Name, ResourceType.Name, ResourcePool.Name); }
    }

    public IResourceType ResourceType
    {
      get { return _model.ResourceType; }
    }

    public IResourcePool ResourcePool
    {
      get { return _model.ResourcePool; }
    }
    public IProductType ProductType
    {
      get { return _model.ProductType; }
    }

    private void ReactToModelChange(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Amount")
      {
        NotifyOfPropertyChange(() => Amount);
      }
    }

    public string DisplayName { get; set; }
  }
}