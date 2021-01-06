using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Emporer.WPF.ViewModels;
using Milan.Simulation;

namespace EcoFactory.Components.UI
{
  public static class Utils
  {
    internal static bool TryRemoveItem<TOwner, TItem>(TOwner owner, ObservableCollection<TOwner> listOfViewModels, Action<TItem> removeFromModel)
      where TOwner : IEditViewModel
    {
      if (!listOfViewModels.Contains(owner))
      {
        return false;
      }
      listOfViewModels.Remove(owner);
      removeFromModel((TItem) owner.Model);
      return true;
    }

    internal static IEnumerable<IProductType> GetAvailableProductTypes(this IEntity entity, IEnumerable<IProductType> productTypesAlreadyInUse)
    {
      return entity.Model.Entities.OfType<IProductType>()
                   .Except(productTypesAlreadyInUse);
    }
  }
}