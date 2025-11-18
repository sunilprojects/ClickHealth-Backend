namespace ClickHealthBackend.DTOs
{
    /// <summary>
    /// Data Transfer Object used by the MR to log observations and feedback from the field.
    /// </summary>
    public class FieldFeedbackDto
    {
        // NOTE: This should be populated from the authenticated user's context (the MR's ID)
        public string MrUserId { get; set; }

        /// <summary>
        /// The main content of the field report or observation.
        /// </summary>
        public string Notes { get; set; }
    }
}
