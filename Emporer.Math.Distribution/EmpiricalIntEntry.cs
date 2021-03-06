﻿#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class EmpiricalIntEntry
  {
    [JsonProperty]
    public int Value { get; set; }

    [JsonProperty]
    public double Frequency { get; set; }
  }
}