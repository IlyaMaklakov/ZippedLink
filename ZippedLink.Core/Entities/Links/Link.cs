using System;
using System.ComponentModel.DataAnnotations.Schema;

using MyCoreFramework.Domain.Entities;
using MyCoreFramework.Domain.Entities.Auditing;

namespace ZippedLink.Core.Entities.Links
{
    [Table("Links")]
    public class Link : Entity<long>, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        public string OriginalUrl { get; set; }

        public string ShortenedUrl { get; set; }
    }
}