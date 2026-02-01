using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Punnawut.IT081.Api.Shared.Repositories;

public class IT081Repositories : IIT081Repositories
{
    private readonly string _connectionString;

    public IT081Repositories(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres not found");
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);

    public async Task<PostDto?> GetPostByIdAsync(long postId)
    {
        const string sql = """
        select
          p.id as Id,
          u.display_name as Author,
          u.avatar_text as AvatarText,
          p.post_time as PostTime,
          p.image_url as ImageUrl
        from posts p
        join users u on u.id = p.author_id
        where p.id = @postId
          and p.is_active = true
        limit 1;
        """;

        using var conn = CreateConnection();
        var post = await conn.QuerySingleOrDefaultAsync<PostDto>(sql, new { postId });
        if (post is null) return null;
        return post;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostIdAsync(long postId)
    {
        const string sql = """
            select
              c.id as Id,
              u.display_name as Author,
              u.avatar_text as AvatarText,
              c.message as Message,
              c.created_at as CreatedAt
            from comments c
            join users u on u.id = c.author_id
            and u.is_active = true
            join posts p
            on p.id = c.post_id
            and p.is_active = true
            where c.post_id = @postId
            and c.is_active = true
            order by c.created_at asc;
        """;

        using var conn = CreateConnection();
        return await conn.QueryAsync<CommentDto>(sql, new { postId });
    }

    public async Task CreateCommentAsync(long postId, string message, string author)
    {
        const string sql = """
        insert into comments(post_id, author_id, message, is_active)
        select
          @postId,
          u.id,
          @message,
          true
        from users u
        join posts p on p.id = @postId and p.is_active = true
        where u.display_name = @author
          and u.is_active = true;
    """;

        using var conn = CreateConnection();
        var affected = await conn.ExecuteAsync(sql, new { postId, message, author });

        if (affected == 0)
            throw new InvalidOperationException("Post or Author not found / inactive");
    }

}

public static class IT081RepositoriesServiceCollectionExtensions
{
    public static IServiceCollection AddIT081Repositories(
        this IServiceCollection services)
    {
        services.AddScoped<IIT081Repositories, IT081Repositories>();
        return services;
    }
}

