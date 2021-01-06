using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emporer.WPF.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  public interface IChainedParameterCommandFactory
  {
    ChainedParameterCommandViewModel CreateAddProductTypeSpecificAmountCommand(string displayText,
                                                                                   ObservableCollection<object> availableProductTypes,
                                                                                   Action<IDictionary<string, object>> addAction);

    ChainedParameterCommandViewModel CreateAddProductTypeSpecificDistributionCommand(string displayText,
                                                                                         ObservableCollection<object> availableProductTypes,
                                                                                         Action<IDictionary<string, object>> addAction);
  }
}