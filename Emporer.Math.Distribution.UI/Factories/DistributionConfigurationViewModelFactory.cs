#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution.Factories;
using Emporer.Math.Distribution.UI.ViewModels;

namespace Emporer.Math.Distribution.UI.Factories
{
  //todo: create dist vm factory foreach dist
  [Export(typeof (IDistributionConfigurationViewModelFactory))]
  internal class DistributionConfigurationViewModelFactory : IDistributionConfigurationViewModelFactory
  {
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;

    [ImportingConstructor]
    public DistributionConfigurationViewModelFactory([ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories)
    {
      _distributionFactories = distributionFactories;
    }

    public IEnumerable<DistributionConfigurationViewModel> CreateViewModelForEachDistributionType()
    {
      return _distributionFactories.Select(df => df.CreateConfiguration())
                                    .Select(SelectAppropriateViewModel);
    }

    public DistributionConfigurationViewModel CreateMatchingViewModel(IDistributionConfiguration model)
    {
      return SelectAppropriateViewModel(model);
    }

    public IEnumerable<DistributionDescriptor> GetDescriptorsOfAllAvailableDistributions()
    {
      //todo: remove distribution descriptor and use factories instead
      return _distributionFactories.Select(df => new DistributionDescriptor(df))
                                    .ToArray();
    }

    private DistributionConfigurationViewModel SelectAppropriateViewModel(IDistributionConfiguration distributionConfiguration)
    {
      if (distributionConfiguration is ConstantDistributionConfiguration)
      {
        return new ConstantDistributionViewModel((ConstantDistributionConfiguration) distributionConfiguration);
      }
      
      if (distributionConfiguration is BernoulliDistributionConfiguration)
      {
        return new BernoulliDistributionViewModel((BernoulliDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is UniformDistributionConfiguration)
      {
        return new UniformDistributionViewModel((UniformDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is LogNormalDistributionConfiguration)
      {
        return new LogNormalDistributionViewModel((LogNormalDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is NormalDistributionConfiguration)
      {
        return new NormalDistributionViewModel((NormalDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is ExponentialDistributionConfiguration)
      {
        return new ExponentialDistributionViewModel((ExponentialDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is WeibullDistributionConfiguration)
      {
        return new WeibullDistributionViewModel((WeibullDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is GammaDistributionConfiguration)
      {
        return new GammaDistributionViewModel((GammaDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is ListDistributionConfiguration)
      {
        return new ListOfValuesDistributionViewModel((ListDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is PoissonDistributionConfiguration)
      {
        return new PoissonDistributionViewModel((PoissonDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is GeometricDistributionConfiguration)
      {
        return new GeometricDistributionViewModel((GeometricDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is ErlangDistributionConfiguration)
      {
        return new ErlangDistributionViewModel((ErlangDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is TriangularDistributionConfiguration)
      {
        return new TriangularDistributionViewModel((TriangularDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is BetaDistributionConfiguration)
      {
        return new BetaDistributionViewModel((BetaDistributionConfiguration) distributionConfiguration);
      }

      if (distributionConfiguration is EmpiricalRealDistributionConfiguration)
      {
        return new EmpiricalRealDistributionViewModel((EmpiricalRealDistributionConfiguration) distributionConfiguration);
      }

      return null;
    }
  }
}