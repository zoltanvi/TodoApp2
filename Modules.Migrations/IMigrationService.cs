using Microsoft.EntityFrameworkCore;

namespace Modules.Migrations
{
    public interface IMigrationService
    {
        void Run(IEnumerable<DbContext> contexts);
    }
}
