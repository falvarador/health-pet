using DinkToPdf;
using DinkToPdf.Contracts;

public class PDFVoucherService : IVoucherService
{
    private readonly IConverter _converter;

    public PDFVoucherService(IConverter converter)
    {
        _converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    public byte[] GenerateVoucher(string head, string body)
    {
        var html = $@"
        <!DOCTYPE html>
        <html lang=""es"">
        <style>
            * {{
                font-family: Arial, Helvetica, sans-serif;
            }}
            head {{
                font-size: 64px;
                font-weight: bold;
            }}
            body {{
                font-size: 64px;
            }}
        </style>
        <head>
            {head}
        </head>
        <body>
            {body}
        </body>
        </html>
        ";

        var htmlToPdfDocument = MakePdfDocument(html);

        return _converter.Convert(htmlToPdfDocument);
    }

    private HtmlToPdfDocument MakePdfDocument(string html)
    {
        var globalSettings = new GlobalSettings()
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A5,
            Margins = new MarginSettings { Top = 25, Bottom = 25 }
        };

        var objectSettings = new ObjectSettings()
        {
            PagesCount = true,
            HtmlContent = html
        };

        var webSettings = new WebSettings()
        {
            DefaultEncoding = "utf-8"
        };

        var headerSettings = new HeaderSettings()
        {
            FontSize = 12,
            FontName = "Helvetica",
            Right = "Page [page] of [toPage]",
            Line = true
        };

        var footerSettings = new FooterSettings()
        {
            FontSize = 10,
            FontName = "Helvetica",
            Center = "This is for demonstration purposes only.",
            Line = true
        };

        objectSettings.HeaderSettings = headerSettings;
        objectSettings.FooterSettings = footerSettings;
        objectSettings.WebSettings = webSettings;

        var htmlToPdfDocument = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings },
        };

        return htmlToPdfDocument;
    }
}
