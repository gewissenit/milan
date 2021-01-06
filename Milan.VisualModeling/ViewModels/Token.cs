#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.VisualModeling.ViewModels
{
  public class Token : Visual, IToken
  {
    public Token(object content)
    {
      Content = content;
    }

    public object Content { get; private set; }
  }
}