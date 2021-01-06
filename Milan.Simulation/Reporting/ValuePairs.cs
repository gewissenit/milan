#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Reporting
{
  public struct StringKeyDoubleValuePair
  {
    public StringKeyDoubleValuePair(string key, double value)
      : this()
    {
      Key = key;
      Value = value;
    }


    public string Key { get; private set; }
    public double Value { get; set; }
  }

  public struct TypeKeyDoubleValuePair
  {
    public TypeKeyDoubleValuePair(Type key, double value)
      : this()
    {
      Key = key;
      Value = value;
    }


    public Type Key { get; private set; }
    public double Value { get; set; }
  }

  public struct StringKeyTimeSpanValuePair
  {
    public StringKeyTimeSpanValuePair(string key, TimeSpan value)
      : this()
    {
      Key = key;
      Value = value;
    }


    public string Key { get; private set; }
    public TimeSpan Value { get; set; }
  }
}