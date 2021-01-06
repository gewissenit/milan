namespace Milan.Simulation.Resources.Factories
{
  interface IInfluenceFactory
  {
    IInfluence Create();
    IInfluence Duplicate(IInfluence master);
  } 
}
