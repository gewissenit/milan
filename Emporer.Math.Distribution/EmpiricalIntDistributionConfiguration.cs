using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Emporer.Math.Distribution
{
  [JsonObject(MemberSerialization.OptIn)]
  public class EmpiricalIntDistributionConfiguration : DistributionConfiguration
  {
    [JsonProperty]
    private readonly IList<EmpiricalIntEntry> _entries = new List<EmpiricalIntEntry>();

    public EmpiricalIntDistributionConfiguration()
    {
      Name = "Empirical";
    }

    public IEnumerable<EmpiricalIntEntry> Entries
    {
      get { return _entries; }
    }


    public void Add(EmpiricalIntEntry entry)
    {
      var existing = _entries.SingleOrDefault(cm => cm.Value == entry.Value);
      if (existing != null)
      {
        existing.Frequency = entry.Frequency;
        return;
      }
      _entries.Add(entry);
    }


    public void Remove(EmpiricalIntEntry entry)
    {
      if (!_entries.Contains(entry))
      {
        throw new InvalidOperationException("The given entry does not exist.");
      }
      _entries.Remove(entry);
    }
  }
}