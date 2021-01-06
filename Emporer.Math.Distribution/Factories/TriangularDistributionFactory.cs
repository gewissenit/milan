using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class TriangularDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Triangular"; }
    }

    public string Description
    {
      get
      {
        return
          "Triangular distributed stream of pseudo random numbers. Values produced by this distribution are triangular distributed in the range specified by parameters.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is TriangularDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new TriangularDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (TriangularDistributionConfiguration) cfg;
      return new TriangularDistributionConfiguration
             {
               Id = master.Id,
               LowerBorder = master.LowerBorder,
               UpperBorder = master.UpperBorder,
               Mean = master.Mean
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (TriangularDistributionConfiguration) cfg;
      var dist = new TriangularDistribution() as ITriangularDistribution;
      dist.LowerBorder = distConfig.LowerBorder;
      dist.UpperBorder = distConfig.UpperBorder;
      dist.Mean = distConfig.Mean;
      return dist as IRealDistribution;
    }

    public IRealDistribution CreateDistribution()
    {
      return new TriangularDistribution();
    }
  }
}