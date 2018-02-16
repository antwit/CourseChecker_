using GemBox.Spreadsheet;
using System;
using System.Data;
using System.Collections.Generic;
using CourseChecker.Course;

namespace CourseChecker.PDF
{
    class CreatePDF
    {
        public CreatePDF(List<Kurse> kurse, String path)
        {
            String date = DateTime.Now.ToShortDateString();
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            ExcelFile ef = new ExcelFile();
            ExcelWorksheet ws = ef.Worksheets.Add("Kursliste " + date);
            DataTable dt = new DataTable();
            ExcelPrintOptions epo = ws.PrintOptions;
            epo.Portrait = false;

            dt.Columns.Add("Kurs-Nr.", typeof(String));
            dt.Columns.Add("Kurs-Name", typeof(String));
            dt.Columns.Add("Beginn", typeof(DateTime));
            dt.Columns.Add("Ende", typeof(DateTime));
            dt.Columns.Add("Ort", typeof(String));
            dt.Columns.Add("Anbieter", typeof(String));
            dt.Columns.Add("Buchungen", typeof(int));
            dt.Columns.Add("Preis", typeof(int));
            dt.Rows.Add(new Object[] { "", "", null, null, "", "", null });

            try
            {
                int j = 0;
                foreach (Kurse kurs in kurse)
                {
                    if (j < 20)
                    {
                        dt.Rows.Add(new Object[] { kurs.StrKursNr, kurs.StrKursTitel, kurs.DateBeginn, kurs.DateEnde, kurs.StrOrt, kurs.StrAnbieter, 0, kurs.IPreis });
                        dt.Rows.Add(new Object[] { "", kurs.StrReason, null, null, "", "", null });
                        dt.Rows.Add(new Object[] { "", "", null, null, "", "", null });
                    }
                    j++;
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Leeres PDF Dokument erzeugt!");
            }

            ws.Rows[0].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells.Style.Font.Size = 9 * 20;
            ws.Columns[6].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Columns[2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Columns[3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Columns[7].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Columns[2].Style.NumberFormat = "[$-409]dd.mm.yyyy";
            ws.Columns[3].Style.NumberFormat = "[$-409]dd.mm.yyyy";
            ws.Columns[7].Style.NumberFormat = "#,##0.00 \"€\"";
            ws.InsertDataTable(dt,
                new InsertDataTableOptions()
                {
                    ColumnHeaders = true,
                    StartRow = 0,
                });

            var columnCount = ws.CalculateMaxUsedColumns();
            for (int i = 0; i < columnCount; i++)
                ws.Columns[i].AutoFit();
            
            ef.Save(path, new PdfSaveOptions() { SelectionType = SelectionType.EntireFile });
        }
    }
}
