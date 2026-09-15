using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Domain.Users;
using KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL.Repositories;

/// <summary>PostgreSQL 用户仓库。</summary>
public sealed class UserRepository(GenocsContext context) : IUserRepository
{
    private readonly GenocsContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.AuthorizationCodes)
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.AuthorizationCodes)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> FindByRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.AuthorizationCodes)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.TokenHash == tokenHash), cancellationToken);

    public async Task<User?> FindByAuthorizationCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.AuthorizationCodes)
            .SingleOrDefaultAsync(u => u.AuthorizationCodes.Any(c => c.Code == code), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
