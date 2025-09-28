using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilder.Data;
using SkillBuilder.Models;

public class CertificateController : Controller
{
    private readonly AppDbContext _context;
    private readonly CertificateService _certificateService;

    public CertificateController(AppDbContext context, CertificateService certificateService)
    {
        _context = context;
        _certificateService = certificateService;
    }

    [HttpGet("/Courses/DownloadCertificate/{courseId}")]
    public async Task<IActionResult> DownloadCertificate(int courseId)
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
            return Unauthorized();

        // Check if user is enrolled and completed
        var enrollment = await _context.Enrollments
            .Include(e => e.User)
            .Include(e => e.Course)
                .ThenInclude(c => c.Artisan)
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

        if (enrollment == null || !enrollment.IsCompleted)
            return BadRequest("You must complete this course before downloading a certificate.");

        var learnerName = $"{enrollment.User.FirstName} {enrollment.User.LastName}";
        var courseTitle = enrollment.Course.Title;
        var artisanName = $"{enrollment.Course.Artisan.FirstName} {enrollment.Course.Artisan.LastName}";
        var artisanSkill = enrollment.Course.Artisan.Profession;

        var pdfBytes = _certificateService.GenerateCertificate(learnerName, courseTitle, artisanName, artisanSkill);

        return File(pdfBytes, "application/pdf", $"Certificate-{learnerName.Replace(" ", "_")}.pdf");
    }
}