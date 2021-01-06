#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Windows.Media;
using Caliburn.Micro;

namespace EcoFactory.Material.Ecoinvent.UI.ViewModels
{
  public class EcoinventMaterialViewModel : PropertyChangedBase
  {
    private string _searchStringCategory;
    private string _searchStringMetaData;
    private string _searchStringName;
    private string _searchStringRegion;
    private string _searchStringSimple;
    private string _searchStringSubcategory;
    private string _iconName;

    public EcoinventMaterialViewModel(MaterialData element, EcoinventBrowserViewModel parentBrowser)
    {
      Element = element;
    }

    public string CategorySearchString
    {
      get { return _searchStringCategory; }
      set
      {
        if (_searchStringCategory == value)
        {
          return;
        }
        _searchStringCategory = value;
        NotifyOfPropertyChange(() => CategorySearchString);
      }
    }

    public MaterialData Element { get; private set; }

    public string NameSearchString
    {
      get { return _searchStringName; }
      set
      {
        if (_searchStringName == value)
        {
          return;
        }
        _searchStringName = value;
        NotifyOfPropertyChange(() => NameSearchString);
      }
    }

    public string Region
    {
      get
      {
        return string.IsNullOrEmpty(Element.Region)
                 ? string.Empty
                 : string.Format("{0}", Element.Region);
      }
    }

    public MaterialType Type
    {
      get { return Element.Type; }
    }

    private bool _alreadyAdded;

    public bool AlreadyAdded
    {
      get { return _alreadyAdded; }
      set
      {
        if (_alreadyAdded == value)
        {
          return;
        }
        _alreadyAdded = value;
        NotifyOfPropertyChange(() => AlreadyAdded);
      }
    }

    public string TypeIcon
    {
      get
      {
        if (_iconName == null)
        {
          switch (Element.Type)
          {
            case MaterialType.Technosphere:
              _iconName ="appbar_gauge_75";
              break;
            case MaterialType.Resource:
              _iconName = "appbar_tree";
              break;
            case MaterialType.Emission:
              _iconName = "Factory";
              break;
          }
        }
        return _iconName;
      }
    }

    public string RegionSearchString
    {
      get { return _searchStringRegion; }
      set
      {
        if (_searchStringRegion == value)
        {
          return;
        }
        _searchStringRegion = value;
        NotifyOfPropertyChange(() => RegionSearchString);
      }
    }

    public string SearchStringMetaData
    {
      get { return _searchStringMetaData; }
      set
      {
        if (_searchStringMetaData == value)
        {
          return;
        }
        _searchStringMetaData = value;
        NotifyOfPropertyChange(() => SearchStringMetaData);
      }
    }

    public string SimpleSearchString
    {
      get { return _searchStringSimple; }
      set
      {
        if (_searchStringSimple == value)
        {
          return;
        }
        _searchStringSimple = value;
        NotifyOfPropertyChange(() => SimpleSearchString);
      }
    }

    public string SubCategorySearchString
    {
      get { return _searchStringSubcategory; }
      set
      {
        if (_searchStringSubcategory == value)
        {
          return;
        }
        _searchStringSubcategory = value;
        NotifyOfPropertyChange(() => SubCategorySearchString);
      }
    }
  }
}