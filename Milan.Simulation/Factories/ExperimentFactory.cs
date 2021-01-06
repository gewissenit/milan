#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Milan.Simulation.Logging;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IExperimentFactory))]
  internal class ExperimentFactory : IExperimentFactory
  {
    private readonly IExperimentLogWriterProvider _logWriterProvider;
    private readonly IModelFactory _modelFactory;

    [ImportingConstructor]
    public ExperimentFactory([Import] IExperimentLogWriterProvider logWriterProvider, [Import] IModelFactory modelFactory)
    {
      _logWriterProvider = logWriterProvider;
      _modelFactory = modelFactory;
    }

    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/></exception>
    public IExperiment Create(IModel model, int seed, IEnumerable<IStatisticalObserverFactory> statisticalObserverFactories, double settlingTime)
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }

      var experiment = new Experiment(seed, settlingTime, _logWriterProvider);
      _modelFactory.CreateSimulationModel(model, statisticalObserverFactories, experiment);
      return experiment;
    }
  }
}