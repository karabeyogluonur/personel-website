namespace PW.Application.Common.Interfaces
{
    public interface IPWDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}


