using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class BetaDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Beta"; }
    }

    public string Description
    {
      get
      {
        return
          "Beta distribution using min, max and median parameters to calculate the two shape parameters of the underlying Gamma. The parameters must follow this condition: ((min <= median) and (median <= max)) and (min != max). The shape values will be calculated by these formulas: Shape1 = 1 + 4 * ((m-a) / (b-a)). Shape2 = 6 - Shape1.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is BetaDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new BetaDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (BetaDistributionConfiguration) cfg;
      return new BetaDistributionConfiguration
             {
               Id = master.Id,
               Maximum = master.Maximum,
               Minimum = master.Minimum,
               FirstShape = master.FirstShape,
               SecondShape = master.SecondShape
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (BetaDistributionConfiguration) cfg;
      return new BetaDistribution
             {
               FirstShape = distConfig.FirstShape,
               SecondShape = distConfig.SecondShape,
               Maximum = distConfig.Maximum,
               Minimum = distConfig.Minimum
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new BetaDistribution();
    }
  }
}