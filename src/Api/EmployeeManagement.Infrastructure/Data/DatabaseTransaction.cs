using System.Threading.Tasks;
using EmployeeManagement.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace EmployeeManagement.Infrastructure.Data
{
    internal class DatabaseTransaction : IDatabaseTransaction
    {
        private readonly EmployeeManagementDbContext _dbContext;

        public DatabaseTransaction(EmployeeManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            return dbContextTransaction;
        }
    }
}
