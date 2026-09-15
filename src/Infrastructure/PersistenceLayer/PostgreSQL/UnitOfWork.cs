using KiriyamaServer.Application.Services;

namespace KiriyamaServer.Infrastructure.PersistenceLayer.PostgreSQL;

public sealed class UnitOfWork(GenocsContext context) : IUnitOfWork, IDisposable
{
    private readonly GenocsContext _context = context;

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}