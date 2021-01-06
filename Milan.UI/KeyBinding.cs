using System;

namespace Milan.UI
{
  internal class KeyBinding
  {
    private readonly Func<bool> _canExecute;
    private readonly Action _execute;

    public KeyBinding(Func<bool> canExecute, Action execute, string name = "")
    {
      if (canExecute == null)
      {
        throw new ArgumentNullException("canExecute");
      }
      if (execute == null)
      {
        throw new ArgumentNullException("execute");
      }
      if (name == null)
      {
        throw new ArgumentNullException("name");
      }
      Name = name;
      _canExecute = canExecute;
      _execute = execute;
    }

    public string Name { get; private set; }

    public void ExecuteIfPossible()
    {
      if (!_canExecute())
      {
        return;
      }
      _execute();
    }
  }
}