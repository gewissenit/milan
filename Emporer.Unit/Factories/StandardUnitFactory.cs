#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Milan.JsonStore;

namespace Emporer.Unit.Factories
{
  [Export(typeof (IStandardUnitFactory))]
  internal class StandardUnitFactory : IStandardUnitFactory
  {
    private const string DataFile = @"Emporer.Unit.Resources.StandardUnits.csv";
    private readonly IJsonStore _store;
    private readonly IUnitFactory _unitFactory;

    [ImportingConstructor]
    public StandardUnitFactory([Import] IJsonStore store, [Import] IUnitFactory unitFactory)
    {
      _store = store;
      _unitFactory = unitFactory;
      _store.ProjectChanged.Subscribe(_ => Initialize());
      Initialize();
    }

    public IEnumerable<IUnit> StandardUnits { get; private set; }

    private void Initialize()
    {
      CreateStandardUnits();
      StandardUnits = _store.Content.OfType<IUnit>()
                            .Where(u => u.IsReadonly)
                            .ToArray();
    }

    private IEnumerable<UnitData> LoadUnitsFromCsv()
    {
      IEnumerable<UnitData> units;
      using (var stream = Assembly.GetExecutingAssembly()
                                  .GetManifestResourceStream(DataFile))
      {
        units = Utils.GetRecordsFromStream<UnitData>(stream, 0, Encoding.UTF8);
      }

      return units;
    }

    private void CreateStandardUnits()
    {
      var units = LoadUnitsFromCsv().ToArray();
      foreach (var unitData in units.Where(ud => !_store.Content.OfType<IUnit>()
                            .Any(u => u.Name == ud.name && u.Symbol == ud.symbol && u.Dimension == ud.dimension && u.IsReadonly)))
      {
        var unit = _unitFactory.Create();
        unit.Dimension = unitData.dimension;
        unit.IsReadonly = true;
        unit.Name = unitData.name;
        unit.Symbol = unitData.symbol;
        unit.Coefficient = unitData.coefficient.GetValueOrDefault();
        
        if (!string.IsNullOrEmpty(unitData.referencedUnit))
        {
          var rud = units.Single(u => u.id == unitData.referencedUnit);
          var referencedUnit = _store.Content.OfType<IUnit>()
                .Single(u => u.Name == rud.name && u.Symbol == rud.symbol && u.Dimension == rud.dimension && u.IsReadonly);
          unit.ReferencedUnit = referencedUnit;
        }
      }
    }
  }
}