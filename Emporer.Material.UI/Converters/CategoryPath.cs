#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Emporer.Material.UI.Converters
{
  public class CategoryPath : IValueConverter
  {
    private const string NullValue = "None"; //TODO: Make translatable using culture parameter
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return NullValue;
      }


      if (!(value is ICategory))
      {
        return null;
      }
      var category = (ICategory) value;

      return ResolvePath(category, string.Empty);
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private string ResolvePath(ICategory category, string basePath)
    {
      var parentCategory = category.ParentCategory;
      if (parentCategory != null)
      {
        basePath = parentCategory.Name + ">";
        return ResolvePath(category.ParentCategory, basePath);
      }
      return basePath;
    }
  }
}