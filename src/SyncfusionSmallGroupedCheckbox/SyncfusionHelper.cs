using Syncfusion.Pdf.Parsing;

namespace SyncfusionSmallGroupedCheckbox;

public class SyncfusionHelper
{
    public static void SetGroupedCheckbox(string pdfTemplateFilePath, string outputFolder)
    {
        ArgumentNullException.ThrowIfNull(pdfTemplateFilePath);
        ArgumentNullException.ThrowIfNull(outputFolder);

        if (!File.Exists(pdfTemplateFilePath))
        {
            throw new FileNotFoundException($"Unable to load PDF template file '{pdfTemplateFilePath}'.");
        }

        // Load the template into a memory stream that we can modify.
        using (MemoryStream pdfTemplateStream = new())
        {
            using (var pdfTemplateFileStream = File.OpenRead(pdfTemplateFilePath))
            {
                pdfTemplateFileStream.CopyTo(pdfTemplateStream);
                pdfTemplateStream.Position = 0L;
            }


            //
            // Set the first grouped checkbox.
            //

            // Create a PDF document for filling form fields.
            using var pdfDocToFill = new PdfLoadedDocument(file: pdfTemplateStream);

            // Grouped checkbox
            if (pdfDocToFill.Form.Fields.TryGetField("firearmApp", out PdfLoadedField groupedField))
            {
                // We know this field is a grouped checkbox. Set the first child checkbox.
                var checkBoxField = (PdfLoadedCheckBoxField)groupedField;
                checkBoxField.Items[0].Checked = true;
            }

            // Non-grouped checkbox
            if (pdfDocToFill.Form.Fields.TryGetField("RaceAsian", out PdfLoadedField nonGroupedField))
            {
                // We know this field is NOT a grouped checkbox. Set the checkbox itself.
                var checkBoxField = (PdfLoadedCheckBoxField)nonGroupedField;
                checkBoxField.Checked = true;
            }


            //
            // Save the file to disk. Write to the specified output directory.
            //

            var outputFilePath = FileHelper.GetOutputFilePath(pdfTemplateFilePath, "syncfusion", outputFolder);
            SaveFile(pdfDocToFill, outputFilePath);


            //
            // Launch the PDF in a new Chrome browser tab.
            //

            ChromeHelper.OpenPdfInBrowser(outputFilePath);
        }
    }

    private static void SaveFile(PdfLoadedDocument pdfDocument, string outputFilePath)
    {
        using (var generatedPdfStream = File.Open(outputFilePath, FileMode.Create, FileAccess.Write))
        {
            pdfDocument.Save(generatedPdfStream);
        }
    }
}
