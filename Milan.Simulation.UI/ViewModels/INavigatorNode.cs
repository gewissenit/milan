using System.Collections.ObjectModel;
using System.ComponentModel;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.ViewModels
{
  public interface INavigatorNode : INotifyPropertyChanged, IViewModel
  {
    bool IsSelected { get; set; }
    bool IsExpanded { get; set; }
    ObservableCollection<INavigatorNode> Items { get; }
    string DisplayName { get; }
  }
}