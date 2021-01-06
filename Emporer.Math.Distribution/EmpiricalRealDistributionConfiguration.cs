using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class EmpiricalRealDistributionConfiguration : DistributionConfiguration
  {
    [JsonProperty]
    private readonly IList<EmpiricalRealEntry> _entries = new List<EmpiricalRealEntry>();

    public EmpiricalRealDistributionConfiguration()
    {
      Name = "Empirical";
    }

    public IEnumerable<EmpiricalRealEntry> Entries
    {
      get { return _entries; }
    }

    public void Add(EmpiricalRealEntry entry)
    {
      var existing = _entries.SingleOrDefault(cm => cm.Value == entry.Value);
      if (existing != null)
      {
        existing.Frequency = entry.Frequency;
        return;
      }
      _entries.Add(entry);
    }

    public void Remove(EmpiricalRealEntry entry)
    {
      if (!_entries.Contains(entry))
      {
        throw new InvalidOperationException("The given entry does not exist.");
      }
      _entries.Remove(entry);
    }
  }
}