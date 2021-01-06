#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.ShiftSystems.UI.ViewModels;
using Milan.Simulation.UI.Factories;

namespace Milan.Simulation.ShiftSystems.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class ShiftManagementEditViewModelFactory : StationaryElementEditViewModelFactory<ShiftManagement>
  {
    [ImportingConstructor]
    public ShiftManagementEditViewModelFactory([Import] IDistributionConfigurationViewModelFactory distributionViewModelFactory)
      : base(distributionViewModelFactory)
    {
    }

    protected override IEditViewModel CreateEditViewModel(ShiftManagement model)
    {
      return new ShiftManagementEditViewModel(model, new Screen[]
                                                     {
                                                       new ShiftManagementSectionViewModel(model)
                                                     });
    }
  }
}