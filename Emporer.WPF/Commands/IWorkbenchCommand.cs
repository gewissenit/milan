using System.Windows.Input;

namespace Emporer.WPF.Commands
{
  public interface IWorkbenchCommand : ICommand
  {
    void Enable();
    void Disable();
  }
}