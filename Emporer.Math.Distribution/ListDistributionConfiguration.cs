#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ListDistributionConfiguration : DistributionConfiguration
  {
    private readonly ObservableCollection<object> _Values = new ObservableCollection<object>();

    public ListDistributionConfiguration()
    {
      Name = "List of values";
      IsPeriodic = true;
    }

    [JsonProperty]
    public ObservableCollection<object> Values
    {
      get { return _Values; }
    }

    [JsonProperty]
    public bool IsPeriodic
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }
    
    public override string ToString()
    {
      if (Values.Count == 0)
      {
        return string.Format("List Of Values");
      }
      return string.Format("lov");
    }
  }
}