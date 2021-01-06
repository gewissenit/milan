namespace Milan.Simulation.Observers
{
  public interface IEntityObserver : ISimulationObserver
  {
    IEntity Entity { get; set; }
  }

  public interface IEntityObserver<TEntity> : IEntityTypeObserver<TEntity>, IEntityObserver
    where TEntity : IEntity
  {
    new TEntity Entity { get; set; }
  }
}