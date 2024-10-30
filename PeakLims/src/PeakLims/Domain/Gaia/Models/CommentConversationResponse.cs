namespace PeakLims.Domain.Gaia.Models;

using System.Text.Json.Serialization;

public record CommentConversationResponse
{
    public List<Comment> Conversation { get; set; } = new();

    public record Comment
    {
        public string CommentText { get; set; } = string.Empty;
        public string? OriginalCommentText { get; set; } = string.Empty;
        public string UserIdentifier { get; set; } = string.Empty;
        public int OrderInConversation { get; set; }
    }
}