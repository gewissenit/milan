namespace EcoFactory.Components.Factories
{
  public interface ITransformationRuleOutputFactory
  {
    ITransformationRuleOutput Create();
    ITransformationRuleOutput Duplicate(ITransformationRuleOutput master);
  }
}