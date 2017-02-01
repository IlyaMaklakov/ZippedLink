using System;
using System.ComponentModel.DataAnnotations.Schema;

using MyCoreFramework.Auditing;
using MyCoreFramework.Domain.Entities;

namespace ZippedLink.Core.Entities.Links
{
    [Table("Links")]
    public class Link : Entity<long>, IHasCreationTime
    {

        public Link()
        {
            CreationTime = DateTime.Now;
        }

        public DateTime CreationTime { get; set; }

        public string OriginalUrl { get; set; }

        public string ShortenedUrl { get; set; }
    }
}