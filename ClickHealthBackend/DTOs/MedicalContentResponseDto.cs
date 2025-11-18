// File: DTOs/MedicalContentResponseDto.cs
using ClickHealthBackend.Enums;

public class MedicalContentResponseDto
{
    public string MedicalName { get; set; }
    public string ContentLanguage { get; set; }
    public string Description { get; set; }
    public string ViewPdf { get; set; }
    public string ViewVideo { get; set; }
    public string Actions { get; set; }  // For frontend buttons (Edit/Delete/View)
    public string ContentId { get;  set; }

    public ContentStatus Status { get; set; }


    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}