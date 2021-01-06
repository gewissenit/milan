#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Material;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.MaterialAccounting.ReportDataProviders
{
  internal static class Utils
  {
    internal static bool FilterProperties(IPropertyType property)
    {
      var id = property.DataSourceId;
      return property.DataSourceId == "UBP06" || id == "IPCC2007";
    }


    internal static IEnumerable<string> GetImpactHeaderGroup(IPropertyType propertyType)
    {
      var headerBase = $"{propertyType.Name} {propertyType.EcoSubCat} {propertyType.EcoCat}";

      return new[]
             {
               $"{headerBase} {Constants.TotalAmountHeaderName}", $"{headerBase} {Constants.AvoidableAmountHeaderName}", $"{headerBase} {Constants.UnitHeaderName}"
             };
    }


    internal static object[] GetImpactValueGroup(IPropertyType propertyType, MaterialPosition b, IEnumerable<IPropertyType> properties)
    {
      var propertyId = propertyType.DataSourceId;
      var propertyTypes = properties as IPropertyType[] ?? properties.ToArray();
      return new object[]
             {
               CalculatePropertyValue(b.Material, b.Total, propertyId, propertyTypes),
               CalculatePropertyValue(b.Material, b.Loss, propertyId, propertyTypes),
               GetPropertyUnit(propertyId, propertyTypes)
             };
    }

    private static double AggregateContainedMaterialPropertyValues(IEnumerable<IPropertyType> properties,
                                                                   IMaterial material,
                                                                   double amount,
                                                                   string propertyId)
    {
      //HACK: 
      var sumOfContainingMaterials = 0d;
      foreach (var contained in material.ContainedMaterials)
      {
        var containedAmount = amount * contained.Amount; //multiply parent amount with contained factor
        var propertyTypes = properties as IPropertyType[] ?? properties.ToArray();
        if (PropertyHasValue(contained.Material, propertyTypes.Single(pt => pt.DataSourceId == propertyId)))
        {
          var factor = GetPropertyValue(contained.Material, propertyId);

          sumOfContainingMaterials += containedAmount * factor;
          // get impact factor for contained, multiply with amount and add to sum
        }
        sumOfContainingMaterials += AggregateContainedMaterialPropertyValues(propertyTypes,
                                                                             contained.Material,
                                                                             containedAmount,
                                                                             propertyId);
        // traverse next level recursively
      }
      return sumOfContainingMaterials;
    }

    private static double CalculatePropertyValue(IMaterial material, double amount, string propertyId, IEnumerable<IPropertyType> properties)
    {
      var factor = GetPropertyValue(material, propertyId);
      var sum = amount * factor;
      sum += AggregateContainedMaterialPropertyValues(properties, material, amount, propertyId);
      return sum;
    }
    
    private static string GetPropertyUnit(string propertyId, IEnumerable<IPropertyType> properties)
    {
      var property = properties.SingleOrDefault(p => p.DataSourceId == propertyId);
      return property == null
               ? string.Empty
               : property.Unit;
    }

    private static double GetPropertyValue(IMaterial material, string propertyId)
    {
      var property = material.Properties.SingleOrDefault(p => p.PropertyType.DataSourceId == propertyId);
      return property == null
               ? 0d
               : property.Mean;
    }

    private static bool PropertyHasValue(IMaterial material, IPropertyType propertyType)
    {
      var property = material.Properties.SingleOrDefault(p => p.PropertyType == propertyType);

      if (property == null)
      {
        return false;
      }

      var isNotZero = Math.Abs(property.Mean - default(double)) > double.Epsilon;
      // supposedly correct way of telling if double!=0
      var isNAN = double.IsNaN(property.Mean);
      var isInfinity = double.IsInfinity(property.Mean);

      return (isNotZero) && !isNAN && !isInfinity;
    }
  }
}