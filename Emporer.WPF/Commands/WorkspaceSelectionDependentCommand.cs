using System;
using System.Collections.Generic;

namespace Emporer.WPF.Commands
{

  public class WorkspaceSelectionDependentCommand<TSubject> : IWorkbenchCommand where TSubject : class
  {
    private ISelection _selection;
    private bool _canExecute;
    private Action<TSubject> _action;

    public WorkspaceSelectionDependentCommand(ISelection selection, Action<TSubject> action)
    {
      _selection = selection;
      _action = action;

      _canExecute = CanExecuteWith(_selection.Current);

      SubscribeToSelectionChanges();
    }


    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object _)
    {
      return _canExecute;
    }

    public void Disable()
    {
      throw new NotImplementedException();
    }

    public void Enable()
    {
      throw new NotImplementedException();
    }

    public void Execute(object _)
    {
      var item = _selection.Current as TSubject;
      if (item != null)
      {
        _action(item);
        return;
      }

      var items = _selection.Current as IEnumerable<TSubject>;
      if (items != null)
      {
        items.ForEach(_action);
        return;
      }
    }

    private bool CanExecuteWith(object currentSelection)
    {
      if (currentSelection==null)
      {
        return false;
      }

      var typeOfSelection = currentSelection.GetType();

      if (typeof(TSubject).IsAssignableFrom(typeOfSelection))
      {
        return true;
      }

      if (typeof(IEnumerable<TSubject>).IsAssignableFrom(typeOfSelection))
      {
        return true;
      }

      return false;
    }

    private void SubscribeToSelectionChanges()
    {
      _selection.Subscribe<object>(this, ExecuteWhenResponsible);
    }

    private void ExecuteWhenResponsible(object selection)
    {
      CanExecuteChanged(this, new EventArgs());
    }
  }
}
