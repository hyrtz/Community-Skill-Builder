using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

public class CertificateService
{
    private readonly string _wwwroot;
    private const string BrandColor = "#754b35"; // Main certificate text color

    public CertificateService(IWebHostEnvironment env)
    {
        _wwwroot = env.WebRootPath ?? throw new ArgumentNullException(nameof(env));
    }

    public byte[] GenerateCertificate(
    string learnerName,
    string courseTitle,
    string artisanName,
    string artisanSkill,
    string? digitalSignatureUrl // <-- pass this URL here
)
    {
        var backgroundPath = Path.Combine(_wwwroot, "assets", "certificate", "certificate-template.png");

        if (!File.Exists(backgroundPath))
            throw new FileNotFoundException("Certificate template not found at: " + backgroundPath);

        var imageBytes = File.ReadAllBytes(backgroundPath);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.PageColor(Colors.White);

                page.Background().Image(imageBytes, ImageScaling.FitArea);

                page.Content().PaddingHorizontal(56).Column(column =>
                {
                    column.Item().PaddingTop(400);

                    column.Item().AlignCenter().Text("THIS CERTIFICATE IS AWARDED TO")
                        .FontSize(10)
                        .SemiBold()
                        .FontColor(BrandColor);

                    column.Item().PaddingTop(18).AlignCenter().Text(learnerName)
                        .FontSize(36)
                        .Bold()
                        .FontColor(BrandColor);

                    column.Item().PaddingTop(12).AlignCenter().Text(
                        $"For successfully completing the {courseTitle} course under the guidance of {artisanName}, {artisanSkill}"
                    )
                    .FontSize(11)
                    .FontColor(BrandColor);

                    column.Item().PaddingTop(160);

                    // Footer: learner info + signature below
                    column.Item().Column(col =>
                    {
                        // Learner info
                        col.Item().Text(learnerName)
                            .FontSize(18)
                            .Bold()
                            .FontColor(BrandColor);

                        col.Item().PaddingTop(8).Element(elem =>
                        {
                            elem.Height(1).Background(BrandColor);
                        });

                        col.Item().PaddingTop(8).Text(artisanName)
                            .FontSize(10)
                            .SemiBold()
                            .FontColor(BrandColor);

                        col.Item().Text(artisanSkill)
                            .FontSize(9)
                            .FontColor(BrandColor);

                        // Signature (if exists) below learner info
                        if (!string.IsNullOrWhiteSpace(digitalSignatureUrl))
                        {
                            var signaturePath = Path.Combine(_wwwroot, digitalSignatureUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                            if (File.Exists(signaturePath))
                            {
                                var signatureBytes = File.ReadAllBytes(signaturePath);
                                col.Item()
                                   .PaddingTop(-120)       // spacing from learner info
                                   .Height(60)           // reasonable height for signature
                                   .AlignLeft()          // align to left
                                   .Image(signatureBytes, ImageScaling.FitArea);
                            }
                        }
                    });

                    column.Item().PaddingBottom(20);
                });
            });
        });

        return doc.GeneratePdf();
    }
}