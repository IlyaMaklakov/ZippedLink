using System.Data.Common;
using System.Data.Entity;

using MyCore.EntityFramework;

using ZippedLink.Core.Entities.Links;

namespace ZippedLink.EntityFramework.EntityFramework
{
    public class ZippedLinkDbContext : AbpDbContext
    {
        public virtual IDbSet<Link> Links { get; set; }



        public ZippedLinkDbContext()
            : base("Default")
        {

        }

        public ZippedLinkDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public ZippedLinkDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}
