#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation
{
  public class ModelConfigurationException : SimulationException
  {
    public ModelConfigurationException(IModel model, IEntity entity, string message, string propertyName)
      : this(model, entity, message)
    {
      PropertyName = propertyName;
    }

    public ModelConfigurationException(IModel model, IEntity entity, string message)
      : base(model, entity, message)
    {
    }

    public string PropertyName { get; private set; }
  }
}