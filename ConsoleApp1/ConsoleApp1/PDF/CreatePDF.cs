using System.Diagnostics;
using GemBox.Spreadsheet;
using System;

namespace CourseChecker.PDF
{
    class CreatePDF
    {
        public CreatePDF()
        {
            String date = DateTime.Now.ToShortDateString();
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            ExcelFile ef = new ExcelFile();
            ExcelWorksheet ws = ef.Worksheets.Add("Kursliste " + date);

            ws.Cells[0, 0].Value = "Kurs-Nr.";
            ws.Cells[0, 1].Value = "Kurs-Name";
            ws.Cells[0, 2].Value = "Beginn";
            ws.Cells[0, 3].Value = "Ende";
            ws.Cells[0, 4].Value = "Ort";
            ws.Cells[0, 5].Value = "Anbieter";
            ws.Cells[0, 6].Value = "Buchungen";
            ws.Cells[0, 7].Value = "Preis";

            ef.Save("Kursliste_" + date +".pdf", new PdfSaveOptions() { SelectionType = SelectionType.EntireFile });
        }
    }
}
