using iTextSharp.text.pdf;

namespace SyncfusionSmallGroupedCheckbox;

public static class iTextSharpHelper
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

            PdfReader? reader = null;
            PdfStamper? stamper = null;

            // Save the file to disk. Write to the specified output directory.
            var outputFilePath = FileHelper.GetOutputFilePath(pdfTemplateFilePath, "itextsharp", outputFolder);
            using (var outputFileStream = File.OpenWrite(outputFilePath))
            {
                try
                {
                    reader = new PdfReader(pdfTemplateStream);
                    stamper = new PdfStamper(reader, outputFileStream);

                    // Set the first checkbox.
                    stamper.AcroFields.SetField("firearmApp", "SAR");

                    stamper.Close();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to fill PDF: {ex}");

                    stamper?.Close();
                    reader?.Close();

                    throw;
                }
            }

            //
            // Launch the PDF in a new Chrome browser tab.
            //

            ChromeHelper.OpenPdfInBrowser(outputFilePath);
        }
    }
}
