using KiriyamaServer.Domain.Users;

namespace KiriyamaServer.Application.Repositories;

/// <summary>
/// 用户仓库（驱动端口）。负责用户账户与刷新令牌的持久化。
/// </summary>
public interface IUserRepository
{
    /// <summary>按邮箱查找用户。</summary>
    Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>按 ID 查找用户。</summary>
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>按刷新令牌 hash 查找其所属用户。</summary>
    Task<User?> FindByRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>按授权码查找其所属用户。</summary>
    Task<User?> FindByAuthorizationCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>新增用户。</summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>更新用户（如发放/撤销刷新令牌）。</summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
