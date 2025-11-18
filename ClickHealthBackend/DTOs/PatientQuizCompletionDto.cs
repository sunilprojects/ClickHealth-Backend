namespace ClickHealthBackend.DTOs
{
    public class PatientQuizCompletionDto
    {
        public string InviteCode { get; set; }
        public string ContentId { get; set; }
        public Dictionary<string, object> QuizResponses { get; set; }
    }
}
