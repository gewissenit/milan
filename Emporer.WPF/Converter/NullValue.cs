#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Emporer.WPF.Converter
{
  public class NullValue
  {
    private const string NullValueText = "None";


    public string Name
    {
      get { return NullValueText; }
    }


    public override string ToString()
    {
      return Name;
    }
  }
}