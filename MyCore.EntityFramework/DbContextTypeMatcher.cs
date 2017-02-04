using MyCoreFramework.Domain.Uow;

namespace MyCore.EntityFramework
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<AbpDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider) 
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}