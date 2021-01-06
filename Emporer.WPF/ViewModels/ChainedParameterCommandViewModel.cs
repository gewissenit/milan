#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Caliburn.Micro;

namespace Emporer.WPF.ViewModels
{
  public class ChainedParameterCommandViewModel : PropertyChangedBase
  {
    private readonly IDictionary<string, object> _chosenValues = new Dictionary<string, object>();
    private readonly Action<IDictionary<string, object>> _commandAction;
    private readonly IList<ParameterSet> _parameterValueSets;
    private string _displayText;

    public ChainedParameterCommandViewModel(IEnumerable<ParameterSet> parameterSets, Action<IDictionary<string, object>> commandAction)
    {
      ValuesForFirstParameter = new ObservableCollection<ChainedParameterValueViewModel>();
      _commandAction = commandAction;
      _parameterValueSets = parameterSets.ToList();
      InitializeParameterValueCache();
      UpdateFirstParameterSet();
    }

    public ObservableCollection<ChainedParameterValueViewModel> ValuesForFirstParameter { get; private set; }

    public string DisplayText
    {
      get { return _displayText; }
      set
      {
        if (_displayText == value)
        {
          return;
        }
        _displayText = value;
        NotifyOfPropertyChange(() => DisplayText);
      }
    }

    public void OnParameterChosen(string parameterId, object value)
    {
      _chosenValues[parameterId] = value;
      _parameterValueSets.Single(x => x.ParameterId == parameterId)
                         .Choose(value);

      if (_chosenValues.Values.Any(v => v == null))
      {
        return;
      }
      _commandAction(_chosenValues);
      UpdateFirstParameterSet();
      InitializeParameterValueCache();
    }

    private void UpdateFirstParameterSet()
    {
      ValuesForFirstParameter.Clear();

      var firstSet = _parameterValueSets.First();
      var nextSets = _parameterValueSets.Skip(1)
                                        .ToArray();
      CreateViewModelsForFirstParameters(firstSet, nextSets);
    }

    private void CreateViewModelsForFirstParameters(ParameterSet firstSet, IEnumerable<ParameterSet> nextSets)
    {
      var firstParameterVmSet =
        firstSet.Parameters.Select(p => new ChainedParameterValueViewModel(firstSet.ParameterId, p, OnParameterChosen, nextSets));

      foreach (var chainedParameterValueViewModel in firstParameterVmSet)
      {
        ValuesForFirstParameter.Add(chainedParameterValueViewModel);
      }

      firstSet.Parameters.CollectionChanged += UpdateFirstParameterSet;
    }

    private void UpdateFirstParameterSet(object sender, NotifyCollectionChangedEventArgs e)
    {
      ((INotifyCollectionChanged) sender).CollectionChanged -= UpdateFirstParameterSet;
      UpdateFirstParameterSet();
    }

    private void InitializeParameterValueCache()
    {
      _chosenValues.Clear();
      foreach (var parameterSet in _parameterValueSets)
      {
        _chosenValues.Add(parameterSet.ParameterId, null);
      }
    }
  }
}