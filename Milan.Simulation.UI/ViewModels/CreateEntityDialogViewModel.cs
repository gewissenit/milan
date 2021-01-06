#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Milan.Simulation.Factories;

namespace Milan.Simulation.UI.ViewModels
{
  public class CreateEntityDialogViewModel : PropertyChangedBase
  {
    public CreateEntityDialogViewModel(IEnumerable<IEntityFactory> entityFactories, IModel model)
    {
      EntityFactories = entityFactories.Select(ef => new EntityFactoryViewModel(ef, model))
                                       .ToArray();
      EntityFactories.ForEach(ef => ef.PropertyChanged += (s, e) =>
                                                          {
                                                            if (e.PropertyName != "Number")
                                                            {
                                                              return;
                                                            }
                                                            NotifyOfPropertyChange(() => NewEntityCount);
                                                            NotifyOfPropertyChange(() => MultipleEntititesSelected);
                                                          });
    }

    public IEnumerable<EntityFactoryViewModel> EntityFactories { get; private set; }

    public int NewEntityCount
    {
      get { return EntityFactories.Sum(de => de.Number); }
    }

    public bool MultipleEntititesSelected
    {
      get { return NewEntityCount > 0; }
    }

    public void AddSelectedEntities()
    {
      foreach (var ef in EntityFactories)
      {
        for (var i = 0; i < ef.Number; i++)
        {
          ef.Create();
        }
      }
    }
  }
}