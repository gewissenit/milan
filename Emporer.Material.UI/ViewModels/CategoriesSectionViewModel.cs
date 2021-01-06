using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace Emporer.Material.UI.ViewModels
{
  public class CategoriesSectionViewModel : Screen
  {
    public CategoriesSectionViewModel(IMaterial model,
                                      IEnumerable<ICategory> categories)
    {
      DisplayName = "categories";
      RootCategories = categories.Where(c => c.ParentCategory == null)
                                 .Select(rc => new SelectedCategoryViewModel(rc,
                                                                             categories,
                                                                             model));
    }

    public IEnumerable<SelectedCategoryViewModel> RootCategories { get; private set; }
  }
}