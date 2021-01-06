namespace Milan.Simulation.Resources.UI.ViewModels
{
  public class ResourcePoolWorkstationViewModel : ResourcePoolViewModel
  {
    private IResourceTypeAmount _selectedResourceTypeAmount;

    public ResourcePoolWorkstationViewModel(IResourcePool resourcePool)
      : base(resourcePool)
    {
    }

    public IResourceTypeAmount SelectedResourceTypeAmount
    {
      get { return _selectedResourceTypeAmount; }
      set
      {
        _selectedResourceTypeAmount = value; 
        NotifyOfPropertyChange(() => SelectedResourceTypeAmount);
      }
    }    

    public int UsageAmount { get; set; }

  }
}