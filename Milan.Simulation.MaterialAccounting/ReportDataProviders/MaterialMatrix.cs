#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Emporer.Material;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.MaterialAccounting.ReportDataProviders
{
  public class MaterialMatrix : ReportDataProvider
  {
    private const string MaterialColumnHeaderText = @"┌- contains # of -→";
    private IMaterial[] _containedMaterials;
    private IMaterial[] _materials;
    private double[][] _matrix;

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      var all = new ReportDataSet
                {
                  Name = "Material Matrix",
                  Description = @"",
                  ColumnHeaders = new[]
                                  {
                                    MaterialColumnHeaderText
                                  }.Concat(_containedMaterials.Select(m => $"{m.Name} ({m.DisplayUnit.Symbol})"))
                                   .ToArray(),
                  Data = CreateRows()
                };

      return new[]
             {
               all
             };
    }

    protected override void Prepare()
    {
      _materials = _source.SelectMany(x => x.Model.Observers)
                          .OfType<IMaterialObserver>()
                          .Select(b => b.Material)
                          .Distinct()
                          .OrderBy(m => m.Name)
                          .ToArray();

      var allMaterials = new List<IMaterial>();

      foreach (var material in _materials)
      {
        GetContainedMaterials(material, ref allMaterials);
      }

      _containedMaterials = allMaterials.OrderBy(m => m.Name)
                                        .ToArray();

      _matrix = new double[_materials.Count()][];

      for (var m = 0; m < _materials.Length; m++)
      {
        _matrix[m] = new double[_containedMaterials.Length];

        for (var c = 0; c < _containedMaterials.Length; c++)
        {
          var material = _materials[m];
          var contained = _containedMaterials[c];

          _matrix[m][c] = GetAmountOfContainingMaterial(material, contained);
        }
      }
    }

    private object[][] CreateRows()
    {
      var result = new object[_materials.Length][]; // +1 because of containing material name

      for (var m = 0; m < _materials.Length; m++)
      {
        result[m] = new object[_containedMaterials.Length + 1]; // +1 because of containing material name
        result[m][0] = string.Format("{0} ({1})", _materials[m].Name, _materials[m].DisplayUnit.Symbol); // first column shows containing material

        for (var c = 0; c < _containedMaterials.Length; c++)
        {
          result[m][c + 1] = GetAmountOfContainingMaterial(_materials[m], _containedMaterials[c]); // writing results beginning in 2nd column
        }
      }

      return result;
    }

    private double GetAmountOfContainingMaterial(IMaterial material, IMaterial contained)
    {
      return !material.ContainedMaterials.Any(c => c.Material == contained)
               ? 0d
               : material.ContainedMaterials.Single(c => c.Material == contained)
                         .Amount;
    }

    private void GetContainedMaterials(IMaterial material, ref List<IMaterial> allMaterials)
    {
      if (allMaterials.Contains(material))
      {
        return;
      }

      allMaterials.Add(material);

      foreach (var containedMaterial in material.ContainedMaterials.Select(c => c.Material))
      {
        GetContainedMaterials(containedMaterial, ref allMaterials);
      }
    }
  }
}