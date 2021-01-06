#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Emporer.WPF.ViewModels
{
  public class ParameterValue
  {
    public ParameterValue(object value, Action<object> gotChosen, Func<object, IEnumerable<ParameterValue>> getNextParameterValues)
    {
      NextParameterValues = getNextParameterValues(value);
      Value = value;
      GotChosen = gotChosen;
    }

    public Action<object> GotChosen { get; private set; }

    public bool IsTerminal
    {
      get { return !NextParameterValues.Any(); }
    }

    public IEnumerable<ParameterValue> NextParameterValues { get; private set; }
    public object Value { get; private set; }

    public void Choose() // in use as caliburn action
    {
      GotChosen(Value);
    }
  }
}