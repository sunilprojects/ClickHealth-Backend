namespace ClickHealthBackend.Enums
{
    public enum CampaignStatus
    {
        Draft,      // Campaign has been created but not yet scheduled to run.
        Active,     // Campaign is currently running (DateTime.UtcNow is between StartDate and EndDate).
        Completed   // Campaign end date has passed.
    }
}
