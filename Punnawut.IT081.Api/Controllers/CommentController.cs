using Microsoft.AspNetCore.Mvc;
using Punnawut.IT081.Api.Shared.Repositories;

namespace Punnawut.IT081.Api.Controllers;

[ApiController]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly IIT081Repositories _repo;

    public CommentController(IIT081Repositories repo)
    {
        _repo = repo;
    }

    // GET /api/comment/post/{id}
    [HttpGet("post/{postId:long}")]
    public async Task<IActionResult> GetPostWithCommentsById(long postId)
    {
        if (postId <= 0)
            return BadRequest(new { message = "Invalid postId" });

        var post = await _repo.GetPostByIdAsync(postId);
        if (post is null)
            return NotFound(new { message = "Post not found" });

        var comments = await _repo.GetCommentsByPostIdAsync(postId);
        return Ok(new { post, comments });
    }

    public record CreateCommentRequest(
     long PostId,
     string Message
 );

    // POST /api/comment/comment/
    // body: { "message": "hello" }
    [HttpPost("comment")]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest req)
    {
        if (req.PostId <= 0)
            return BadRequest(new { message = "PostId is required" });

        if (string.IsNullOrWhiteSpace(req.Message))
            return BadRequest(new { message = "Message is required" });

        await _repo.CreateCommentAsync(
            req.PostId,
            req.Message.Trim(),
            "Blend 285"
        );

        return StatusCode(StatusCodes.Status201Created);
    }


}
