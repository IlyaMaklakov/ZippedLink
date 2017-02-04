using System.Data.Entity;
using System.Reflection;

using MyCore.EntityFramework;

using MyCoreFramework.Modules;
using ZippedLink.Core;
using ZippedLink.EntityFramework.EntityFramework;

namespace ZippedLink.EntityFramework
{
    [DependsOn(typeof(ZippedLinkCoreModule), typeof(AbpEntityFrameworkModule))]
    public class ZippedLinkDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ZippedLinkDbContext>());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
