#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.ObjectModel;

namespace Emporer.WPF.ViewModels
{
  public class ParameterSet
  {
    private readonly Action<object> _onParameterGotChosen;

    public ParameterSet(string parameterId,
                        string displayText,
                        ObservableCollection<object> parameters,
                        Action<object> onParameterGotChosen = null)
    {
      _onParameterGotChosen = onParameterGotChosen;
      ParameterId = parameterId;
      DisplayText = displayText;
      Parameters = parameters;
    }

    public string DisplayText { get; set; }
    public string ParameterId { get; private set; }
    public ObservableCollection<object> Parameters { get; private set; }

    public void Choose(object chosen)
    {
      if (!Parameters.Contains(chosen))
      {
        throw new ArgumentException("Given object is not a parameter of the set and can not be chosen.");
      }

      _onParameterGotChosen?.Invoke(chosen);
    }
  }
}