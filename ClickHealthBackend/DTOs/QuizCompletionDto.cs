namespace ClickHealthBackend.DTOs
{
    public class QuizCompletionDto
    {
        public string HcpUserId { get; set; } // HCP logging their own quiz completion
        public Dictionary<string, object> QuizResponses { get; set; }
    }
}
