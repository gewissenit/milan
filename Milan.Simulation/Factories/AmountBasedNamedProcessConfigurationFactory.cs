using System.ComponentModel.Composition;

namespace Milan.Simulation.Factories
{
  [Export(typeof(INamedProcessConfigurationFactory))]
  public class AmountBasedNamedProcessConfigurationFactory : NamedProcessConfigurationFactory
  {
    public AmountBasedNamedProcessConfigurationFactory()
      : base(() => new AmountBasedNamedProcessConfiguration(), "Amount Based")
    {
    }
  }
}