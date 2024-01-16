using Microsoft.EntityFrameworkCore;

namespace LinqExpressionParser.AspNetCore.Sample.Data;

public class SearchSampleContext : DbContext
{
    public DbSet<User> User { get; set; }

    public SearchSampleContext(DbContextOptions<SearchSampleContext> options)
        : base(options)
    {
    }
}
