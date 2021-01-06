using System.Collections.Generic;

namespace Milan.Simulation.Resources
{
  public interface IInfluenceAware
  {
    IEnumerable<IInfluenceRate> Influences { get; }
    void AddInfluence(IInfluenceRate influence);
    void RemoveInfluence(IInfluenceRate influence);
  }
}