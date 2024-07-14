using Dapper;
using MySqlConnector;

namespace Domain;

public class PostRepository(string? connectionString) : IDisposable
{
    private readonly MySqlConnection _db = new(connectionString);

    public async Task<Post?> GetPostRandomAsync()
    {
        var sql = @"
SELECT 
    PostId,
    Slug,
    Title,
    Year,
    Developer,
    Version,
    Language,
    Tags,
    Size,
    Flag
FROM posts WHERE Flag IS FALSE ORDER BY RAND() LIMIT 1
";
        return await _db.QuerySingleOrDefaultAsync<Post>(sql);
    }

    public async Task<int> SetFlagTrueAsync(Post post)
    {
        var sql = @"UPDATE posts set Flag = TRUE WHERE PostId = @PostId";
        return await _db.ExecuteAsync(sql, new
        {
            PostId = post.PostId
        });
    }

    public void Dispose()
    {
        _db.Close();
    }
}