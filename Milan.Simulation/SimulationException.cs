#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation
{
  public class SimulationException : Exception
  {
    public SimulationException(IModel model, IEntity entity, string message)
      : base(message + " (" + entity.Name + ")")
    {
      Model = model;
      Entity = entity;
    }

    public IModel Model { get; private set; }
    public IEntity Entity { get; set; }
  }
}