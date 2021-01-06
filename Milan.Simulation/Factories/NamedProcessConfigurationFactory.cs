using System;

namespace Milan.Simulation.Factories
{
  public abstract class NamedProcessConfigurationFactory : INamedProcessConfigurationFactory
  {
    private readonly Func<INamedProcessConfiguration> _create;
    private readonly string _name;

    protected NamedProcessConfigurationFactory(Func<INamedProcessConfiguration> create, string name)
    {
      _create = create;
      _name = name;
    }

    public INamedProcessConfiguration Create()
    {
      return _create();
    }

    public string Name
    {
      get { return _name; }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}