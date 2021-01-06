using Milan.Simulation;

namespace EcoFactory.Components.Factories
{
  public interface ITransformationRuleFactory
  {
    ITransformationRule Create();
    ITransformationRule Duplicate(ITransformationRule master);
  }
}