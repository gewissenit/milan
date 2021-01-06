#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table.PivotTable;

namespace Milan.Simulation.Reporting
{
  public static class ExcelExport
  {
    private const string ColumnHeaderStyle = "ColumnHeader";
    
    private static void AutoFilter(ExcelWorksheet worksheet)
    {
      var range = ExcelCellBase.GetAddress(3, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column);
      worksheet.Cells[range].AutoFilter = true;
    }

    private static void AutoFitColumns(ExcelWorksheet worksheet)
    {
      worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
    }

    private static void FormatDateTime(ExcelRange cell, object value)
    {
      cell.Style.Numberformat.Format = Constants.DateFormat;
      cell.Value = (DateTime) value;
    }

    private static void FormatTimeSpan(ExcelRange cell, object value)
    {
      var timespan = (TimeSpan) value;
      var ticks = timespan.Ticks;

      if (ticks > DateTime.MaxValue.Ticks)
      {
        timespan = TimeSpan.MaxValue;
      }

      cell.Value = timespan;
      cell.Style.Numberformat.Format = Constants.TimeSpanFormat;
    }

    private static void SetFormatValueAccordingToDataType(ExcelRange cell, object value)
    {
      if (value is TimeSpan)
      {
        FormatTimeSpan(cell, value);
      }

      else if (value is DateTime)
      {
        FormatDateTime(cell, value);
      }

      else
      {
        cell.Value = value;
      }
    }

    public static void WriteDataSetsToExcel(FileInfo newFile, IEnumerable<IReportDataProvider> reportDataProvider)
    {
      using (var package = new ExcelPackage(newFile))
      {
        var columnHeaderStyle = package.Workbook.Styles.CreateNamedStyle(ColumnHeaderStyle);
        columnHeaderStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        columnHeaderStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        columnHeaderStyle.Style.Font.Bold = true;
        columnHeaderStyle.Style.Font.SetFromFont(new Font("Arial Black", 8));
        columnHeaderStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        columnHeaderStyle.Style.Border.Left.Style = ExcelBorderStyle.Thin;

        foreach (var dataSet in reportDataProvider.SelectMany(rdp => rdp.DataSets))
        {
          // add a new worksheet to the workbook
          var dataSetName = dataSet.Name;
          var worksheet = package.Workbook.Worksheets.Add(dataSetName);

          WriteTitle(dataSet, worksheet);
          WriteHeaders(worksheet, dataSet);
          WriteRows(worksheet, dataSet);
          AutoFilter(worksheet);
          AutoFitColumns(worksheet);

          var startColumn = 1;
          const int startRow = 1;
          foreach (var pv in dataSet.Pivots)
          {
            var source = package.Workbook.Worksheets[dataSetName];
            var rowCount = source.Dimension.End.Row;
            var columnCount = source.Dimension.End.Column;
            var range = source.Cells[pv.StartRow, 1, rowCount, columnCount];
            var overviewName = $"{dataSetName} - Overview";
            ExcelWorksheet overview;
            if (package.Workbook.Worksheets.All(ws => ws.Name != overviewName))
            {
              overview = package.Workbook.Worksheets.Add(overviewName);
              package.Workbook.Worksheets.MoveToStart(overview.Name);
            }
            else
            {
              overview = package.Workbook.Worksheets[overviewName];
            }

            var pivot = overview.PivotTables.Add(overview.Cells[startRow, startColumn], range, pv.Name);
            pivot.UseAutoFormatting = true;
            pivot.DataOnRows = false;
            pivot.DataCaption = pv.Name;
            pivot.RowHeaderCaption = "Categories";
            pivot.RowGrandTotals = !pv.NoGrandTotals;
            foreach (var fieldName in pv.RowFieldNames)
            {
              pivot.RowFields.Add(pivot.Fields[fieldName]);
            }
            foreach (var dataField in pv.DataFields)
            {
              var row = pivot.DataFields.Add(pivot.Fields[dataField.SourceName]);
              row.Name = dataField.Name;
              row.Format = dataField.Format;
              DataFieldFunctions result;
              Enum.TryParse(dataField.Function.ToString(), out result);
              row.Function = result;
            }
            startColumn += pv.DataFields.Count() + 2;
          }
        }
        package.Save();
      }
    }

    private static void WriteHeaders(ExcelWorksheet worksheet, ReportDataSet dataSet)
    {
      var col = 1;
      foreach (var columnHeader in dataSet.ColumnHeaders)
      {
        var headerCell = worksheet.Cells[3, col++];
        headerCell.Value = columnHeader;
        headerCell.StyleName = ColumnHeaderStyle;
      }
    }

    private static void WriteRows(ExcelWorksheet worksheet, ReportDataSet dataSet)
    {
      var row = 3;
      foreach (var record in dataSet.Data)
      {
        row++;
        if (row > 1048576)
        {
          break;
        }
        var col = 1;
        foreach (var value in record)
        {
          var cell = worksheet.Cells[row, col++];
          cell.Value = value;
          SetFormatValueAccordingToDataType(cell, value);
        }
      }
    }

    private static void WriteTitle(ReportDataSet dataSet, ExcelWorksheet worksheet)
    {
      worksheet.Cells[1, 1].Value = dataSet.Name;
      worksheet.Cells[2, 1].Value = dataSet.Description;
    }
  }
}