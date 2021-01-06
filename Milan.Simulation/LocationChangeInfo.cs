#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation
{
  public struct LocationChangeInfo
  {
    private readonly string _location;
    private readonly double _time;
    private readonly LocationChange _type;

    public LocationChangeInfo(string location, double time, LocationChange type)
    {
      _location = location;
      _time = time;
      _type = type;
    }

    public string Location
    {
      get { return _location; }
    }

    public double Time
    {
      get { return _time; }
    }

    public LocationChange Type
    {
      get { return _type; }
    }
  }
}