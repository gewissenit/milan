namespace Milan.Simulation.Factories
{
  public interface IAdditionalEntityDuplicationAction
  {
    void DuplicateEntity(IModel destinationModel, IEntity original, IEntity clone);
  }
}