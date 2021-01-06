#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Observers;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.CostAccounting.ReportDataProviders
{
  public class CostBalances : ReportDataProvider
  {
    private IEnumerable<Position> _balancePositions;

    protected override IEnumerable<ReportDataSet> CreateDataSets()
    {
      return new[]
             {
               new ReportDataSet
               {
                 Name = "Costs",
                 Description = @"",
                 ColumnHeaders = new[]
                                 {
                                   Constants.StationHeaderName, Constants.ProcessHeaderName, Constants.ProductStatusHeaderName, Constants.ProductTypeHeaderName, Constants.CategoryHeaderName, Constants.CurrencyHeaderName, Constants.TotalCostsHeaderName, Constants.AvoidableCostsHeaderName, Constants.ProductHeaderName, Constants.ExperimentHeaderName, Constants.EventHeaderName
                                 },
                 Data = _balancePositions.Select(bp =>
                                                 {
                                                   var ptName = bp.ProductType == null
                                                                  ? ""
                                                                  : bp.ProductType.Name;
                                                   return new object[]
                                                          {
                                                            bp.Station.Name, bp.Process, bp.ProductStatus, ptName, bp.Category, bp.Currency.Symbol, bp.Total, bp.Loss, bp.Product, bp.Experiment.Id, bp.Event
                                                          };
                                                 })
                                         .ToArray(),
                 Pivots = new[]
                          {
                            new Pivot
                            {
                              Name = "Stations",
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
                              Name = "Products",
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
                            }
                          }
               }
             };
    }

    protected override void Prepare()
    {
      _balancePositions = _source.SelectMany(e => e.Model.Observers.OfType<IEventCostObserver>()
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
  }
}