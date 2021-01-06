using Emporer.WPF.ViewModels;
using Milan.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcoFactory.Components.UI.ViewModels
{
  internal class MultipleEntitiesSelectionViewModel : IEditViewModel
  {
    private readonly IEntity[] _selectedElements;
    private readonly string _text;

    public MultipleEntitiesSelectionViewModel(IEnumerable<IEntity> selectedElements)
    {
      _selectedElements = selectedElements.ToArray();
      _text= $"${_selectedElements.Length} elements";
    }

    public string DisplayName
    {
      get
      {
        return _text;
      }

      set { }
    }

    public object Model
    {
      get
      {
        return _selectedElements;
      }
    }
  }
}
