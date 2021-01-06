using System.ComponentModel;
using Caliburn.Micro;
using Emporer.WPF;

namespace Milan.Simulation.UI.ViewModels
{
  public interface IPropertyEditorViewModel : INotifyPropertyChanged
  {
    ISelection Selection { get; set; }
  }
}