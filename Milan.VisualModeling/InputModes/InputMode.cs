#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using ReactiveUI;

namespace Milan.VisualModeling.InputModes
{
  public abstract class InputMode : ReactiveObject, IInputMode
  {
    public InputMode(VisualEditor visualEditor)
    {
      VisualEditor = visualEditor;
    }

    protected VisualEditor VisualEditor { get; private set; }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
      VisualEditor.ChangeCursor(null);
    }
  }
}