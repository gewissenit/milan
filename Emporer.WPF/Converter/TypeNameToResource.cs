#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Emporer.WPF.Converter
{
  /// <summary>
  ///   Takes an arbitrary object (1st bound value) and a <see cref="FrameworkElement" />(2nd bound value) and returns a
  ///   resource that has a key that matches the tye of the object.
  ///   If a format string is provided as converter parameter, the search for a matching resource can be extended with a
  ///   prefix and/or suffix.
  ///   <remarks>
  ///     The converter searches for the resource using the name of the objects type, all inhertited types and all inherited
  ///     interfaces (in that order).
  ///     The search is done in the resource dictionary of the given FrameworkElement and the resource dictionaries of the
  ///     parent visual tree of the element.
  ///     If the optional converter parameter is in the format "[prefix]{0}[suffix]" the search for a resource can be
  ///     expanded to a pattern, e.g.:
  ///   </remarks>
  ///   <example>
  ///     Assuming:
  ///     * Datacontext.Item is of type Foo:Bar
  ///     * TypeNameToResourceConverter is referenced as static resource 'TNTR'
  ///     <Image Source="{Binding Item, Converter={StaticResource TNTR}, ConverterParameter='New{0}Icon'}" />
  ///     This will search for the following resources:
  ///     NewFooIcon
  ///     NewBarIcon
  ///   </example>
  /// </summary>
  public class TypeNameToResource : IMultiValueConverter
  {
    private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

    //TODO: This can be a hack or feature...
    /*
     * Without calling this method before a test run, unit tests that check for different resource keys on the same subject (type) fail.
     * All but the first test won't get the correct resource because the first one 'compromises' the cache.
     * 
     * I keep this for now because caching seams necessary. We could also use this later to clear the cache for long running apps (?).
     */

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      // check values:
      // 2 inputs, domain entity and view
      if (values == null)
      {
        return DependencyProperty.UnsetValue;
      }

      if (values.Count() < 2)
      {
        return DependencyProperty.UnsetValue;
      }

      if (values[0] == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var type = values[0].GetType();

      var fwElement = values[1] as FrameworkElement;

      if (fwElement == null)
      {
        return DependencyProperty.UnsetValue;
      }

      // check parameter
      var format = "{0}";
      var s = parameter as string;
      if (s != null)
      {
        format = s;
      }

      // check cache
      if (_cache.ContainsKey(type.Name))
      {
        return _cache[type.Name];
      }

      // check directly
      var res = GetResourceMatchingTypeName(type.Name, fwElement, format);
      if (res != null)
      {
        _cache.Add(type.Name, res);
        return res;
      }

      // search inheritance tree
      res = GetResourceMatchingInheritedTypeName(type, fwElement, format);
      if (res != null)
      {
        _cache.Add(type.Name, res);
        return res;
      }

      // search interfaces
      res = GetResourceMatchingInterfaceName(type, fwElement, format);
      if (res != null)
      {
        _cache.Add(type.Name, res);
        return res;
      }

      // nothing found at all
      _cache.Add(type.Name, DependencyProperty.UnsetValue);
      return DependencyProperty.UnsetValue;
    }
    
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public static void ClearCache()
    {
      _cache.Clear();
    }


    private static object GetResourceMatchingInheritedTypeName(Type type, FrameworkElement fwElement, string format)
    {
      if (type.BaseType == null)
      {
        return null; // we reached System.Object --> exit condition
      }

      var baseType = type.BaseType;
      var res = GetResourceMatchingTypeName(baseType.Name, fwElement, format);

      if (res != null)
      {
        return res;
      }

      // step down until System.Object is reached
      return GetResourceMatchingInheritedTypeName(baseType, fwElement, format);
    }


    private static object GetResourceMatchingInterfaceName(Type type, FrameworkElement fwElement, string format)
    {
      // search list of all inherited interfaces (no tree here!)
      foreach (var inheritedInterface in type.GetInterfaces())
      {
        var interfaceName = inheritedInterface.Name;

        // cut leading 'I' off the name, assumes correct interface names
        if (interfaceName.StartsWith("I") &&
            interfaceName.Length > 1 &&
            Char.IsUpper(interfaceName[1]))
        {
          interfaceName = interfaceName.TrimStart('I');
        }

        var res = GetResourceMatchingTypeName(interfaceName, fwElement, format);
        if (res != null)
        {
          return res;
        }
      }

      return null;
    }

    private static object GetResourceMatchingTypeName(string typeName, FrameworkElement fwElement, string format)
    {
      var searchString = string.Format(format, typeName);

      // search whole accessible resource dictionary tree
      var res = fwElement.TryFindResource(searchString);
      if (res != null)
      {
        return res;
      }
      return null;
    }
  }
}