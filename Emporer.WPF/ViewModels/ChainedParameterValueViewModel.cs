#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Emporer.WPF.ViewModels
{
  public class ChainedParameterValueViewModel
  {
    private readonly Action<string, object> _onGotSelected;

    private ChainedParameterValueViewModel(string parameterId, object value, Action<string, object> onGotSelected)
    {
      ParameterId = parameterId;
      Value = value;
      _onGotSelected = onGotSelected;
      ValuesForNextParameter = new ChainedParameterValueViewModel[0];
    }

    public ChainedParameterValueViewModel(string parameterId, object value, Action<string, object> onChoose, IEnumerable<ParameterSet> nextParameters)
      : this(parameterId, value, onChoose)
    {
      nextParameters = nextParameters.ToArray();

      if (!nextParameters.Any())
      {
        return;
      }

      var firstSet = nextParameters.First();
      nextParameters = nextParameters.Skip(1);

      ValuesForNextParameter = firstSet.Parameters.Select(p => new ChainedParameterValueViewModel(firstSet.ParameterId, p, onChoose, nextParameters));
    }

    public string ParameterId { get; set; }
    public object Value { get; set; }
    public IEnumerable<ChainedParameterValueViewModel> ValuesForNextParameter { get; private set; }

    public void GetChosen()
    {
      _onGotSelected(ParameterId, Value);
    }
  }
}