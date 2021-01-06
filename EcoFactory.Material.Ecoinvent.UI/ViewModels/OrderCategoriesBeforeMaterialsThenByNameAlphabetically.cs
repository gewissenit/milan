using System;
using System.Collections.Generic;
using System.Globalization;
using Emporer.Material;
using Emporer.WPF.ViewModels;

namespace EcoFactory.Material.Ecoinvent.UI.ViewModels
{
  public class OrderCategoriesBeforeMaterialsThenByNameAlphabetically : IComparer<IViewModel>
  {
    public int Compare(IViewModel x, IViewModel y)
    {

      if (x == y)
      {
        return 0;
      }

      if (x == null &&
          y != null)
      {
        return -1;
      }

      if (x != null &&
          y == null)
      {
        return 1;
      }

      if (x == null &&
          y == null)
      {
        return 0;
      }

      if (x.Model == y.Model)
      {
        return 0;
      }

      if (x.Model is IContainedMaterial && y.Model is IContainedMaterial)
      {
        var cmX = (IContainedMaterial) x.Model;
        var cmY = (IContainedMaterial) y.Model;
        if (cmX.Material == null || cmY.Material == null)
        {
          return 0;
        }
        return string.Compare(cmX.Material.Name, cmY.Material.Name, false, CultureInfo.InvariantCulture);
      }

      var catX = x.Model as ICategory;
      var catY = y.Model as ICategory;
      IMaterial matY;

      if (catX == null)
      {
        var matX = x.Model as IMaterial;
        if (matX == null)
        {
          throw new ArgumentException("First agument is not a view model of a category or a material.");
        }

        // x is material vm
        if (catY != null)
        {
          return 1;
        }

        matY = y.Model as IMaterial;
        if (matY == null)
        {
          throw new ArgumentException("Second agument is not a view model of a category or a material.");
        }

        // y is material vm
        return string.Compare(matX.Name, matY.Name, false, CultureInfo.InvariantCulture);

        // y is category vm
      }

      // x is category vm
      if (catY != null)
      {
        return string.Compare(catX.Name, catY.Name, false, CultureInfo.InvariantCulture);
      }

      matY = y.Model as IMaterial;
      if (matY == null)
      {
        throw new ArgumentException("Second agument is not a view model of a category or a material.");
      }
      // y is material vm
      return -1;

      // y is category vm
    }
  }
}