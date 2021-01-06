using System.ComponentModel.Composition;

namespace Milan.Simulation.Factories
{
  [Export(typeof(INamedProcessConfigurationFactory))]
  public class TimeBasedNamedProcessConfigurationFactory : NamedProcessConfigurationFactory
  {
    public TimeBasedNamedProcessConfigurationFactory()
      : base(() => new TimeBasedNamedProcessConfiguration(), "Time Based")
    {
    }
  }
}