#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Emporer.Material;
using Milan.JsonStore;
using Milan.Simulation.Observers;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.MaterialAccounting.ReportDataProviders
{
  public class MaterialBalances : ReportDataProvider
  {
    private readonly IJsonStore _store;
    private IEnumerable<MaterialPosition> _balancePositions;
    private IEnumerable<IPropertyType> _ecoinventImpactFactors;

    public MaterialBalances(IJsonStore store)
    {
      _store = store;
    }

    private IEnumerable<IPropertyType> EcoinventImpactFactors => _ecoinventImpactFactors ?? (_ecoinventImpactFactors = _store.Content.OfType<IPropertyType>()
                                                                                                                             .Where(Utils.FilterProperties)
                                                                                                                             .ToArray());

    protected override void Prepare()
    {
      _balancePositions = _source.SelectMany(e => e.Model.Observers.OfType<IMaterialObserver>()
                                                   .Where(eco => eco.IsEnabled)
                                                   .SelectMany(o => o.BalancePositions))
                                 .ToArray();
      SetProductStatus();
    }

    private void SetProductStatus()
    {
      foreach (var experiment in _source)
      {
        var observer = experiment.Model.Observers.OfType<ThroughputObserver>()
                                 .Single();
        foreach (var balancePosition in _balancePositions.Where(bp => bp.Experiment == experiment)
                                                         .Where(bp => bp.Product != -1))
        {
          if (observer.RemainingProducts.Contains(balancePosition.Product))
          {
            balancePosition.ProductStatus = Constants.RemainingState;
          }
          else if (observer.DestroyedProducts.Contains(balancePosition.Product))
          {
            balancePosition.ProductStatus = Constants.DestroyedState;
          }
          else if (observer.SuccessfulProducts.Contains(balancePosition.Product))
          {
            balancePosition.ProductStatus = Constants.SuccessfulState;
          }
          else
          {
            balancePosition.ProductStatus = Constants.TransformedState;
          }
        }
      }
    }

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Materials",
                 Description = @"",
                 ColumnHeaders = new[]
                                 {
                                   Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.ProductStatusHeaderName, Constants.ProductTypeHeaderName, Constants.CategoryHeaderName, Constants.MaterialHeaderName, Constants.UnitHeaderName, Constants.TotalAmountHeaderName, Constants.AvoidableAmountHeaderName, Constants.CurrencyHeaderName, Constants.TotalCostsHeaderName, Constants.AvoidableCostsHeaderName, Constants.ProductHeaderName, Constants.ExperimentHeaderName, Constants.EventHeaderName
                                 }.Concat(EcoinventImpactFactors.SelectMany(Utils.GetImpactHeaderGroup))
                                  .ToArray(),
                 Data = _balancePositions.Select(bp =>
                                                 {
                                                   var ptName = bp.ProductType == null
                                                                  ? ""
                                                                  : bp.ProductType.Name;
                                                   var currency = bp.Material.Currency == null
                                                                    ? ""
                                                                    : bp.Material.Currency.Symbol;

                                                   return new object[]
                                                          {
                                                            bp.Station.Name, bp.Process, bp.ProductStatus, ptName, bp.Category, bp.Material.Name, bp.Material.DisplayUnit.BasicUnit.Symbol, bp.Total, bp.Loss, currency, bp.TotalCosts, bp.LossCosts, bp.Product, bp.Experiment.Id, bp.Event
                                                          }.Concat(EcoinventImpactFactors.SelectMany(p => Utils.GetImpactValueGroup(p, bp, EcoinventImpactFactors)))
                                                           .ToArray();
                                                 })
                                         .ToArray(),
                 Pivots = new[]
                          {
                            new Pivot
                            {
                              Name = "Stations - Costs",
                              StartRow = 3,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.SumTotalHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.SumAvoidableHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.MinTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.AvgTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.MaxTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.MinAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.AvgAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.MaxAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             }
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.CurrencyHeaderName, Constants.ExperimentHeaderName, Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.ProductTypeHeaderName, Constants.CategoryHeaderName
                                              }
                            },
                            new Pivot
                            {
                              Name = "Products - Costs",
                              StartRow = 3,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.SumTotalHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.SumAvoidableHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.MinTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.AvgTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalCostsHeaderName,
                                               Name = Constants.MaxTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.MinAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.AvgAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableCostsHeaderName,
                                               Name = Constants.MaxAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             }
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.CurrencyHeaderName, Constants.ExperimentHeaderName, Constants.ProductStatusHeaderName, Constants.ProductTypeHeaderName, Constants.StationHeaderName, Constants.CategoryHeaderName
                                              }
                            },
                            new Pivot
                            {
                              Name = "Stations - Amounts",
                              StartRow = 3,
                              NoGrandTotals = true,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.SumTotalHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.SumAvoidableHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.MinTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.AvgTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.MaxTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.MinAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.AvgAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.MaxAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             }
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.UnitHeaderName, Constants.ExperimentHeaderName, Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.MaterialHeaderName, Constants.ProductTypeHeaderName, Constants.CategoryHeaderName
                                              }
                            },
                            new Pivot
                            {
                              Name = "Products - Amounts",
                              StartRow = 3,
                              NoGrandTotals = true,
                              DataFields = new[]
                                           {
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.SumTotalHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.SumAvoidableHeaderName,
                                               Format = Constants.DoubleFormat
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.MinTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.AvgTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.TotalAmountHeaderName,
                                               Name = Constants.MaxTotalHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.MinAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Min
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.AvgAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Average
                                             },
                                             new DataField
                                             {
                                               SourceName = Constants.AvoidableAmountHeaderName,
                                               Name = Constants.MaxAvoidableHeaderName,
                                               Format = Constants.DoubleFormat,
                                               Function = DataField.Functions.Max
                                             },
                                           },
                              RowFieldNames = new[]
                                              {
                                                Constants.UnitHeaderName, Constants.ExperimentHeaderName, Constants.ProductStatusHeaderName, Constants.ProductTypeHeaderName, Constants.MaterialHeaderName, Constants.StationHeaderName, Constants.CategoryHeaderName
                                              }
                            }
                          }
               }
             };
    }
  }
}