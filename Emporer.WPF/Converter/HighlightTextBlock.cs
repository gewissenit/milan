#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace Emporer.WPF.Converter
{
  public sealed class HighlightTextBlock : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var text = string.Empty;
      var selection = string.Empty;
      var caseSensitive = false;

      // default style
      var style = new Style(typeof (TextBlock));
      style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
      style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(0)));

      if (values.Length > 0)
      {
        text = values[0] as string;
      }

      if (values.Length > 1)
      {
        selection = values[1] as string;
      }

      if (values.Length > 2 &&
          values[2] is Style)
      {
        style = (Style) values[2];
      }

      if (parameter is bool)
      {
        caseSensitive = (bool) parameter;
      }

      var stringComparison = caseSensitive
                               ? StringComparison.Ordinal
                               : StringComparison.OrdinalIgnoreCase;

      string formatedText;

      text = SecurityElement.Escape(text); // fix possible unescaped xml elements in the string

      if (string.IsNullOrEmpty(text) ||
          string.IsNullOrEmpty(selection) ||
          !text.Contains(selection, stringComparison))
      {
        formatedText = text;
      }
      else
      {
        var sb = new StringBuilder();

        while (text.Contains(selection, stringComparison))
        {
          var selectionStartPosition = text.IndexOf(selection, StringComparison.OrdinalIgnoreCase);
          var before = text.Substring(0, selectionStartPosition);
          var matchedText = text.Substring(selectionStartPosition, selection.Length);
          var selectionEndPosition = selectionStartPosition + selection.Length;
          text = text.Substring(selectionEndPosition, text.Length - selectionEndPosition);
          sb.Append(before);
          sb.Append("<Run Background=\"#FFC014\">");
          sb.Append(matchedText);
          sb.Append("</Run>");
        }
        sb.Append(text);
        formatedText = sb.ToString();
      }

      var wrappedInput =
        string.Format(
                      "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">{0}</TextBlock>",
                      formatedText);

      using (var stringReader = new StringReader(wrappedInput))
      {
        using (var xmlReader = XmlReader.Create(stringReader))
        {
          var result = (TextBlock) XamlReader.Load(xmlReader);
          result.Style = style;
          return result;
        }
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}