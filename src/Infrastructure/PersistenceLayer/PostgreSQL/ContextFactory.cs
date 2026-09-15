using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;

public sealed class ContextFactory : IDesignTimeDbContextFactory<GenocsContext>
{
    public GenocsContext CreateDbContext(string[] args)
    {
        string connectionString = ReadConnectionString();

        var builder = new DbContextOptionsBuilder<GenocsContext>();
        builder.UseNpgsql(connectionString);

        return new GenocsContext(builder.Options);
    }

    /// <summary>
    /// 读取连接字符串。优先从环境变量 <c>ConnectionStrings__DefaultConnection</c>
    /// （由 launchSettings.json 注入），未设置时回退到本地默认 PostgreSQL 连接串。
    /// </summary>
    private static string ReadConnectionString()
        => Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=kiriyama;Username=kiriyama;Password=kiriyama";
}