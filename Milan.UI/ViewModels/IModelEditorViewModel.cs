using Caliburn.Micro;
using Emporer.WPF;
using Milan.Simulation;

namespace Milan.UI.ViewModels
{
  public interface IModelEditorViewModel:IScreen
  {
    ISelection Selection { get; set; }
    IModel SelectedModel { get; set; }
  }
}