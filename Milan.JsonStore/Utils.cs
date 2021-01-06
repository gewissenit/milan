#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Milan.JsonStore
{
  public static class Utils
  {
    public static string GetUniqueName(string name, IEnumerable<string> otherNames)
    {
      if (otherNames == null)
      {
        return name;
      }

      otherNames = otherNames.ToArray();
      if (!otherNames.Any())
      {
        return name;
      }

      while (otherNames.Contains(name))
      {
        name = GetIncrementedName(name);
      }

      return name;
    }
    
    public static T[] GetRecordsFromFile<T>(string pathToFile, int ignoreFirstLines)
    {
      #region pre

      if (string.IsNullOrEmpty(pathToFile))
      {
        throw new ArgumentNullException("fileInfo"); //NON-NLS-1
      }

      #endregion

      var engine = new FileHelperEngine(typeof (T));
      engine.Options.IgnoreFirstLines = ignoreFirstLines;
      var records = engine.ReadFile(pathToFile) as T[];
      return records;
    }


    public static T[] GetRecordsFromStream<T>(Stream stream, int ignoreFirstLines, Encoding encoding)
    {
      #region pre

      if (stream == null)
      {
        throw new ArgumentNullException("fileInfo"); //NON-NLS-1
      }

      #endregion

      var reader = new StreamReader(stream, encoding);
      var engine = new FileHelperEngine(typeof (T), encoding);
      engine.Options.IgnoreFirstLines = ignoreFirstLines;
      var records = engine.ReadStream(reader) as T[];
      return records;
    }
    
    public static string GetIncrementedName(string name)
    {
      var opening = name.LastIndexOf('(');
      var closing = name.LastIndexOf(')');

      if (opening < closing &&
          opening > 0 &&
          opening > 0) // 2nd and 3rd bool checks existence of parenthesis (-1 means no)
      {
        // looks like it was incremented before, but we should be sure of it
        var numberPart = name.Substring(opening + 1, closing - opening - 1); // isolate number
        var namePart = name.Substring(0, opening - 1); // isolate name

        // text in (last) parenthesis could be something other than a number
        int result;
        if (!int.TryParse(numberPart, out result))
        {
          return string.Format("{0} (1)", name);
        }

        result++;
        return string.Format("{0} ({1})", namePart, result);
      }

      // incremented for the first time
      return string.Format("{0} (1)", name);
    }
  }
}