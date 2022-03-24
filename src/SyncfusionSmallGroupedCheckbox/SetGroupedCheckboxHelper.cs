using System.Diagnostics;
using Syncfusion.Pdf.Parsing;

namespace SyncfusionSmallGroupedCheckbox;

public class SetGroupedCheckboxHelper
{
    public static void SetGroupedCheckbox(string pdfTemplateFilePath)
    {
        ArgumentNullException.ThrowIfNull(pdfTemplateFilePath);

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

            if (pdfDocToFill.Form.Fields.TryGetField("firearmApp", out PdfLoadedField field))
            {
                // We know this field is a grouped checkbox. Set the first child checkbox.
                var checkBoxField = (PdfLoadedCheckBoxField)field;

                checkBoxField.Items[1].Checked = true;
            }


            //
            // Save the file to disk. Write the filled PDF to the same directory as the template file.
            //

            var outputFilePath = GetOutputFilePath(pdfTemplateFilePath);
            SaveFile(pdfDocToFill, outputFilePath);


            //
            // Launch the PDF in a new Chrome browser tab.
            //

            using Process process = new();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "chrome";
            process.StartInfo.Arguments = outputFilePath;
            process.Start();
        }
    }

    private static string GetOutputFilePath(string inputFilePath)
    {
        var fileNameNoExtension = Path.GetFileNameWithoutExtension(inputFilePath);
        var extension = Path.GetExtension(inputFilePath);
        var templateFileDir = Path.GetDirectoryName(inputFilePath)!;

        return Path.Combine(templateFileDir, $"{fileNameNoExtension}_filled{extension}");
    }

    private static void SaveFile(PdfLoadedDocument pdfDocument, string outputFilePath)
    {
        using (var generatedPdfStream = File.Open(outputFilePath, FileMode.Create, FileAccess.Write))
        {
            pdfDocument.Save(generatedPdfStream);
        }
    }
}
