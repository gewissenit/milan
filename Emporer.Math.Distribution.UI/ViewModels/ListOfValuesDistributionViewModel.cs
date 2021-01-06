#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Emporer.WPF.Commands;
using Emporer.WPF.ViewModels;
using Microsoft.Win32;
using Milan.JsonStore;

namespace Emporer.Math.Distribution.UI.ViewModels
{
  public class ListOfValuesDistributionViewModel : DistributionConfigurationViewModel
  {
    private readonly RelayCommand _addCurrentValueToListCommand;
    private readonly ListDistributionConfiguration _distributionConfiguration;
    private readonly RelayCommand _removeSelectedValueFromListCommand;
    private double _currentValue;
    private bool _isPeriodic;
    private int _selectedIndex;

    public ListOfValuesDistributionViewModel(ListDistributionConfiguration configuration)
      : base(configuration)
    {
      _distributionConfiguration = configuration;

      _addCurrentValueToListCommand = new RelayCommand(_ =>
                                                        {
                                                          _distributionConfiguration.Values.Add(CurrentValue.Value);
                                                          NotifyOfPropertyChange(() => Values);
                                                        },
                                                        _ => CurrentValue != null);

      _removeSelectedValueFromListCommand = new RelayCommand(_ =>
                                                              {
                                                                _distributionConfiguration.Values.RemoveAt(SelectedIndex);
                                                                NotifyOfPropertyChange(() => Values);
                                                              },
                                                              CanRemoveSelectedValueFromList);

      CurrentValue = new DoublePropertyWrapper(() => _currentValue, SetCurrentValue);
    }

    public DoublePropertyWrapper CurrentValue { get; private set; }

    public IEnumerable<double> Values
    {
      get { return _distributionConfiguration.Values.Cast<double>(); }
    }

    public int SelectedIndex
    {
      get { return _selectedIndex; }
      set
      {
        _selectedIndex = value;
        NotifyOfPropertyChange(() => SelectedIndex);
        _removeSelectedValueFromListCommand.EvaluateCanExecute(SelectedIndex);
      }
    }
    
    public bool IsPeriodic
    {
      get { return _isPeriodic; }
      set
      {
        if (_isPeriodic == value)
        {
          return;
        }
        _isPeriodic = value;
        NotifyOfPropertyChange(() => IsPeriodic);
      }
    }
    
    public ICommand AddCurrentValueToList
    {
      get { return _addCurrentValueToListCommand; }
    }

    public ICommand RemoveSelectedValueFromList
    {
      get { return _removeSelectedValueFromListCommand; }
    }

    public ICommand ImportValuesFromCsv
    {
      get
      {
        return new ActionCommand(_ =>
                                 {
                                   var ofd = new OpenFileDialog();

                                   if (ofd.ShowDialog() == false)
                                   {
                                     return;
                                   }

                                   var values = Utils.GetRecordsFromFile<ListDistributionRecord>(ofd.FileName, 1);

                                   foreach (var listOfValueRecord in values)
                                   {
                                     _distributionConfiguration.Values.Add(listOfValueRecord.SecondsToNextOccurrence);
                                   }

                                   NotifyOfPropertyChange(() => Values);
                                 });
      }
    }

    private bool CanRemoveSelectedValueFromList(object something)
    {
      var can = SelectedIndex >= 0 && SelectedIndex < Values.Count();
      return can;
    }

    private void SetCurrentValue(double value)
    {
      _currentValue = value;
      _addCurrentValueToListCommand.EvaluateCanExecute(_currentValue);
    }
  }
}