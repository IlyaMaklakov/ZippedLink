using System;

using MyCoreFramework.Dependency;

namespace MyCore.EntityFramework.Repositories
{
    public interface IEntityFrameworkGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager);
    }
}