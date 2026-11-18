
using Microsoft.EntityFrameworkCore;

namespace TaskManagementPro.Data
{
    public class PostgreSqlDbContext : ApplicationDbContext
    {
        public PostgreSqlDbContext(
            DbContextOptions<PostgreSqlDbContext> options)
            : base(options)
        {
        }
    }
}
