#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.UI.ViewModels;
using Milan.VisualModeling.Persistence;
using Milan.VisualModeling.ViewModels;

namespace Milan.UI.Factories
{
  [Export(typeof (IVisualModelingViewModelFactory))]
  internal class VisualModelingViewModelFactory : IVisualModelingViewModelFactory
  {
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public VisualModelingViewModelFactory([Import] IJsonStore store)
    {
      //TODO: remove dependency to store when we found another way of storing the persistent viewmodel properties
      _store = store;
    }

    public IEdge CreateEdge(INode source, INode destination)
    {
      return new VisualModelingConnection(source, destination);
    }


    public INode CreateNode(IEntity entity)
    {
      // This assumes there is always exactly one instance of ModelConfiguration amongst the content of the store.
      // But is isn't. So who is responsible for adding it in the first place?
      var modelCfg = _store.Content.OfType<ModelConfiguration>()
                           .SingleOrDefault(x => x.Model == entity.Model);

      if (modelCfg == null)
      {
        //INFO: I am here. Fuck. Why.
        throw new InvalidOperationException();
      }

      var nodeCfg = modelCfg.Visuals.SingleOrDefault(x => x.Model == entity);
      if (nodeCfg == null)
      {
        nodeCfg = new VisualConfiguration(entity)
                  {
                    X = 10,
                    Y = 10
                  };
        modelCfg.Visuals.Add(nodeCfg);
      }

      var node = new Node(Create(entity), nodeCfg);

      return node;
    }

    public IViewModel Create(IEntity entity)
    {
      var vm = new VisualModelingEntityViewModel(entity);
      return vm;
    }
  }
}