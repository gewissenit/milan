#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace EcoFactory.Material.Ecoinvent.UI.Converters
{
  public class FlagSourceFromRegion : IValueConverter
  {
    private const string None = "none";
    private const string EuropeanUnion = "europeanunion";
    private const string World = "world";
    private const string Us = "us";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var regionString = value as string;
      if (string.IsNullOrEmpty(regionString))
      {
        regionString = None;
      }

      var baseUri = parameter as string ?? string.Empty;

      var flagname = SwitchFlagName(regionString);


      return string.Format("{0}{1}.png", baseUri, flagname);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private string SwitchFlagName(string regionString)
    {
      switch (regionString)
      {
        case "ASCC":
          return Us;
        case "CENTREL":
          return EuropeanUnion;
        case "CPA":
          return None;
        case "EEU":
          return EuropeanUnion;
        case "ERCOT":
          return Us;
        case "FRCC":
          return Us;
        case "GLO":
          return World;
        case "MRO":
          return Us;
        case "NORDEL":
          return EuropeanUnion;
        case "NPCC":
          return Us;
        case "OCE":
          return None;
        case "RAF":
          return None;
        case "RAS":
          return None;
        case "RER":
          return EuropeanUnion;
        case "RFC":
          return Us;
        case "RLA":
          return None;
        case "RME":
          return None;
        case "RNA":
          return None;
        case "SERC":
          return Us;
        case "SPP":
          return Us;
        case "UCTE":
          return EuropeanUnion;
        case "WECC":
          return Us;
        case "WEU":
          return EuropeanUnion;

        default:
          return regionString.ToLower();
      }
    }
  }
}