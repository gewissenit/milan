#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using EcoFactory.Material.Ecoinvent.Factories;
using Emporer.Material;
using Milan.JsonStore;
using ReactiveUI;

namespace EcoFactory.Material.Ecoinvent.UI.ViewModels
{
  [Export]
  public class EcoinventBrowserViewModel : ReactiveObject
  {
    private readonly IEcoinventMaterialFactory _ecoinventMaterialFactory;
    private readonly ObservableAsPropertyHelper<IEnumerable<EcoinventMaterialViewModel>> _matchingElements;
    private readonly EcoinventMaterialViewModel[] _materials;
    private readonly ObservableAsPropertyHelper<int> _numberOfMatchingElements;
    private bool _isSearchCaseSensitive;
    private MaterialType _materialTypeFilter = MaterialType.NotSpecified;

    private string _searchStringCategory = string.Empty;
    private string _searchStringMetaData = string.Empty;
    private string _searchStringName = string.Empty;
    private string _searchStringRegion = string.Empty;
    private string _searchStringSubcategory = string.Empty;

    [ImportingConstructor]
    public EcoinventBrowserViewModel([Import] IEcoinventMaterialFactory ecoinventMaterialFactory, [Import] IJsonStore store)
    {
      _ecoinventMaterialFactory = ecoinventMaterialFactory;
      _materials = ecoinventMaterialFactory.EcoInventMaterials.Select(ef => new EcoinventMaterialViewModel(ef, this))
                                           .OrderBy(ef => ef.Element.Name)
                                           .ToArray();
      var storeMaterials = store.Content.OfType<IMaterial>()
                                .Where(m => m.IsReadonly)
                                .ToArray();

      store.ItemRemoved.Subscribe(x =>
                                  {
                                    var material = x as IMaterial;
                                    if (material == null ||
                                        !material.IsReadonly)
                                    {
                                      return;
                                    }
                                    var matchingMaterialViewModel = GetMatchingEcoinventMaterial(material);
                                    if (matchingMaterialViewModel != null)
                                    {
                                      matchingMaterialViewModel.AlreadyAdded = false;
                                    }
                                  });
      foreach (var material in storeMaterials)
      {
        var matchingMaterialViewModel = GetMatchingEcoinventMaterial(material);
        if (matchingMaterialViewModel != null)
        {
          matchingMaterialViewModel.AlreadyAdded = true;
        }
      }

      Regions = _materials.Select(m => m.Element.Region)
                          .Distinct()
                          .ToArray();
      Categories = _materials.Select(m => m.Element.Category)
                             .Distinct()
                             .ToArray();
      Subcategories = _materials.Select(m => m.Element.SubCategory)
                                .Distinct()
                                .ToArray();

      var availableItemsChanged = this.WhenAny(x => x.SearchStringName,
                                               x => x.SearchStringCategory,
                                               x => x.SearchStringSubcategory,
                                               x => x.SearchStringRegion,
                                               x => x.SearchStringMetaData,
                                               x => x.IsSearchCaseSensitive,
                                               x => x.MaterialTypeFilter,
                                               (n, c, s, r, m, t, _) => Filter())
                                      .Throttle(TimeSpan.FromMilliseconds(500));

      availableItemsChanged.ToProperty(this, x => x.MatchingElements, out _matchingElements);
      availableItemsChanged.Select(x => x.Count())
                           .ToProperty(this, x => x.NumberOfMatchingElements, out _numberOfMatchingElements);
    }

    public IEnumerable<string> Regions { get; private set; }
    public IEnumerable<string> Categories { get; private set; }
    public IEnumerable<string> Subcategories { get; private set; }

    public IEnumerable<EcoinventMaterialViewModel> MatchingElements
    {
      get { return _matchingElements.Value; }
    }

    public object Model { get; private set; }

    public int NumberOfElements
    {
      get { return _materials.Length; }
    }

    public int NumberOfMatchingElements
    {
      get { return _numberOfMatchingElements.Value; }
    }

    public bool IsSearchCaseSensitive
    {
      get { return _isSearchCaseSensitive; }
      set { this.RaiseAndSetIfChanged(ref _isSearchCaseSensitive, value); }
    }

    public MaterialType MaterialTypeFilter
    {
      get { return _materialTypeFilter; }
      set { this.RaiseAndSetIfChanged(ref _materialTypeFilter, value); }
    }

    public string SearchStringCategory
    {
      get { return _searchStringCategory; }
      set { this.RaiseAndSetIfChanged(ref _searchStringCategory, value); }
    }

    public string SearchStringName
    {
      get { return _searchStringName; }
      set { this.RaiseAndSetIfChanged(ref _searchStringName, value); }
    }

    public string SearchStringRegion
    {
      get { return _searchStringRegion; }
      set { this.RaiseAndSetIfChanged(ref _searchStringRegion, value); }
    }

    public string SearchStringSubcategory
    {
      get { return _searchStringSubcategory; }
      set { this.RaiseAndSetIfChanged(ref _searchStringSubcategory, value); }
    }

    public string SearchStringMetaData
    {
      get { return _searchStringMetaData; }
      set { this.RaiseAndSetIfChanged(ref _searchStringMetaData, value); }
    }

    private EcoinventMaterialViewModel GetMatchingEcoinventMaterial(IMaterial m)
    {
      return _materials.SingleOrDefault(mvm => m.Name == mvm.Element.Name && m.Description == mvm.Element.MetaData && m.Categories.First()
                                                                                                                       .Name ==
                                               mvm.Element.SubCategory && m.Categories.First()
                                                                           .ParentCategory.Name == mvm.Element.Category &&
                                               m.DisplayUnit.Symbol == mvm.Element.Unit &&
                                               ((!m.Properties.Any() && !mvm.Element.Ubp06.HasValue && !mvm.Element.Ipcc2007.HasValue) ||
                                                (m.Properties.First()
                                                  .Mean == mvm.Element.Ubp06 && m.Properties.Last()
                                                                                 .Mean == mvm.Element.Ipcc2007) ||
                                                (m.Properties.Count() == 1 && (m.Properties.First()
                                                                                .Mean == mvm.Element.Ubp06 || m.Properties.First()
                                                                                                               .Mean == mvm.Element.Ipcc2007))));
    }

    public IEnumerable<EcoinventMaterialViewModel> Filter()
    {
      var searchResult = _materials.Where(MatchesCurrentFilter)
                                   .OrderBy(x => x.Element.Name);

      return searchResult;
    }

    public void Import(EcoinventMaterialViewModel material)
    {
      _ecoinventMaterialFactory.ImportMaterials(new[]
                                                {
                                                  material.Element
                                                });
      material.AlreadyAdded = true;
    }

    private bool Matches(string subject, string filter)
    {
      var stringComparison = IsSearchCaseSensitive
                               ? StringComparison.Ordinal
                               : StringComparison.OrdinalIgnoreCase;
      if (string.IsNullOrEmpty(filter))
      {
        return true;
      }
      // split the search phrase and for every word
      var words = filter.Split(@"\s");
      return words.All(word => subject.Contains(word, stringComparison));
    }

    private bool MatchesCurrentFilter(EcoinventMaterialViewModel materialViewModel)
    {
      var material = materialViewModel.Element;

      var isOfSelectedType = new Func<MaterialData, bool>(x =>
                                                          {
                                                            if (MaterialTypeFilter == MaterialType.NotSpecified)
                                                            {
                                                              return true; // matches all
                                                            }
                                                            return x.Type == MaterialTypeFilter;
                                                          });

      var result = Matches(material.Name, SearchStringName) && Matches(material.Category, SearchStringCategory) &&
                   Matches(material.SubCategory, SearchStringSubcategory) && Matches(material.Region, SearchStringRegion) &&
                   Matches(material.MetaData, SearchStringMetaData) && isOfSelectedType(material);

      return result;
    }
  }
}