using System.Data;

namespace Punnawut.IT081.Api.Shared.Repositories;

public interface IIT081Repositories
{
    IDbConnection CreateConnection();

    Task<PostDto?> GetPostByIdAsync(long postId);
    Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(long postId);
    Task CreateCommentAsync(long postId, string message, string author);
}

public class PostDto
{
    public long Id { get; init; }
    public string Author { get; init; } = "";
    public string? AvatarText { get; init; }
    public DateTime PostTime { get; init; }
    public string? ImageUrl { get; set; }
}


public record CommentDto(
    long Id,
    string Author,
    string AvatarText,
    string Message,
    DateTime CreatedAt
);
