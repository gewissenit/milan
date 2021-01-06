namespace Emporer.Math.Distribution.Factories
{
  public interface IDistributionFactory<T>
    where T : IDistribution
  {
    string Name { get; }
    string Description { get; }
    bool CanHandle(IDistributionConfiguration cfg);
    IDistributionConfiguration CreateConfiguration();
    IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg);
    T CreateAndConfigureDistribution(IDistributionConfiguration cfg);
    T CreateDistribution();
  }
}