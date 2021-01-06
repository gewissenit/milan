#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Emporer.Math.Distribution.UI.Factories;
using Emporer.WPF.Factories;
using Emporer.WPF.ViewModels;
using Milan.Simulation.Resources.UI.ViewModels;
using Milan.Simulation.UI.Factories;

namespace Milan.Simulation.Resources.UI.Factories
{
  [Export(typeof(IEditViewModelFactory))]
  internal class ResourceTypeEditViewModelFactory : StationaryElementEditViewModelFactory<ResourceType>
  {
    [ImportingConstructor]
    public ResourceTypeEditViewModelFactory([Import] IDistributionConfigurationViewModelFactory distributionViewModelFactory)
      : base(distributionViewModelFactory)

    {
    }

    protected override IEditViewModel CreateEditViewModel(ResourceType model)
    {
      return new ResourceTypeEditViewModel(model,
                                           new Screen[]
                                           {
                                             new InfluencesSectionViewModel(model), new MaintenanceSectionViewModel(model,
                                                                                                                    DistributionViewModelFactory)
                                           });
    }
  }
}