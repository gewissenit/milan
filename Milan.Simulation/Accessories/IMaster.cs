#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Accessories
{
  public interface IMaster : IEntity
  {
    void Cooperate(ISlave slave);
  }
}