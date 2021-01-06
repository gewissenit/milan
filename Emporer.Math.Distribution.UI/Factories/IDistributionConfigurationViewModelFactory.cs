using System.Collections.Generic;
using Emporer.Math.Distribution.UI.ViewModels;

namespace Emporer.Math.Distribution.UI.Factories
{
  public interface IDistributionConfigurationViewModelFactory
  {
    /// <summary>
    ///   Creates a view model for each available type of distribution. The view model will wrap a newly created instance of
    ///   the specific distribution configuration type.
    /// </summary>
    /// <returns>An instance of a matching view model for each available distribution configuration type.</returns>
    IEnumerable<DistributionConfigurationViewModel> CreateViewModelForEachDistributionType();

    /// <summary>
    ///   Creates a matching view model for the given distribution configuration. The view model will wrap the given
    ///   distribution configuration instance.
    /// </summary>
    /// <param name="model">A distribution configuration.</param>
    /// <returns></returns>
    DistributionConfigurationViewModel CreateMatchingViewModel(IDistributionConfiguration model);

    //TODO: this should rather be a member of some IDistributionConfigurationFactory
    IEnumerable<DistributionDescriptor> GetDescriptorsOfAllAvailableDistributions();
  }
}