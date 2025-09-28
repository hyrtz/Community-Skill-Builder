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

    public byte[] GenerateCertificate(string learnerName, string courseTitle, string artisanName, string artisanSkill)
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

                // Background template
                page.Background().Image(imageBytes, ImageScaling.FitArea);

                // Overlay content
                page.Content().PaddingHorizontal(56).Column(column =>
                {
                    column.Item().PaddingTop(400);

                    // Subtitle
                    column.Item().AlignCenter().Text("THIS CERTIFICATE IS AWARDED TO")
                        .FontSize(10)
                        .SemiBold()
                        .FontColor(BrandColor);

                    // Big learner name
                    column.Item().PaddingTop(18).AlignCenter().Text(learnerName)
                        .FontSize(36)
                        .Bold()
                        .FontColor(BrandColor);

                    // Description
                    column.Item().PaddingTop(12).AlignCenter().Text(
                        $"For successfully completing the {courseTitle} course under the guidance of {artisanName}, {artisanSkill}"
                    )
                    .FontSize(11)
                    .FontColor(BrandColor);

                    column.Item().PaddingTop(160);

                    // Footer / signature row
                    column.Item().Row(row =>
                    {
                        row.RelativeColumn().Column(left =>
                        {
                            left.Item().Text(learnerName)
                                .FontSize(18)
                                .Bold()
                                .FontColor(BrandColor);

                            left.Item().PaddingTop(8).Element(elem =>
                            {
                                elem.Height(1).Background(BrandColor);
                            });

                            left.Item().PaddingTop(8).Text(artisanName)
                                .FontSize(10)
                                .SemiBold()
                                .FontColor(BrandColor);

                            left.Item().Text(artisanSkill)
                                .FontSize(9)
                                .FontColor(BrandColor);
                        });

                        row.ConstantColumn(180).Column(right =>
                        {
                            right.Item().AlignRight().Text(""); // placeholder
                        });
                    });

                    column.Item().PaddingBottom(20);
                });
            });
        });

        return doc.GeneratePdf();
    }
}