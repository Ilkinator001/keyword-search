using DocumentFormat.OpenXml.Packaging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System.Text;

namespace FileSearch
{
    internal class FileReader
    {
        public string GetContentFromFile(string filePath)
        {
            //var content = File.ReadAllText(filePath);

            //return content;

            return GetContentFromWindowsFile(filePath);
        }

        private string GetContentFromWindowsFile(string filePath)
        {
            // Prüfen auf Datei-Erweiterung
            var extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".txt":
                    // Standard-Textdatei
                    return File.ReadAllText(filePath);

                case ".docx":
                    // Word-Dokument (.docx)
                    return ExtractTextFromWord(filePath);

                case ".xlsx":
                    // Excel-Datei (.xlsx)
                    return ExtractTextFromExcel(filePath);

                case ".pptx":
                    // PowerPoint-Datei (.pptx)
                    return ExtractTextFromPowerPoint(filePath);

                default:
                    // Unbekanntes Dateiformat - keine Suche
                    throw new ArgumentException("Unknown file format.");
            }
        }

        // Extrahiert Text aus einer Word-Datei (.docx)
        private string ExtractTextFromWord(string filePath)
        {
            var text = new StringBuilder();
            using (var stream = File.OpenRead(filePath))
            {
                var doc = new XWPFDocument(stream);
                foreach (var paragraph in doc.Paragraphs)
                {
                    text.AppendLine(paragraph.Text);
                }
            }
            return text.ToString();
        }

        // Extrahiert Text aus einer Excel-Datei (.xlsx)
        private string ExtractTextFromExcel(string filePath)
        {
            var text = new StringBuilder();
            using (var stream = File.OpenRead(filePath))
            {
                var workbook = new XSSFWorkbook(stream);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var sheet = workbook.GetSheetAt(i);
                    foreach (IRow row in sheet) // Typisierung auf IRow
                    {
                        foreach (var cell in row.Cells) // Zugriff auf Cells funktioniert nun
                        {
                            text.Append(cell.ToString() + "\t");
                        }
                        text.AppendLine();
                    }
                }
            }
            return text.ToString();
        }

        // Extrahiert Text aus einer PowerPoint-Datei (.pptx)
        private string ExtractTextFromPowerPoint(string filePath)
        {
            var text = new StringBuilder();
            using (var presentation = PresentationDocument.Open(filePath, false))
            {
                var slideParts = presentation.PresentationPart.SlideParts;
                foreach (var slidePart in slideParts)
                {
                    var paragraphs = slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>();
                    foreach (var paragraph in paragraphs)
                    {
                        text.AppendLine(paragraph.InnerText);
                    }
                }
            }
            return text.ToString();
        }
    }
}
