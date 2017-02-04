using System;
using System.IO;

using Microsoft.Extensions.FileProviders;

using MyCoreFramework.Resources.Embedded;

namespace MyCore.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceItemFileInfo : IFileInfo
    {
        public bool Exists => true;

        public long Length => this._resourceItem.Content.Length;

        public string PhysicalPath => null;

        public string Name => this._resourceItem.FileName;

        public DateTimeOffset LastModified => this._resourceItem.LastModifiedUtc;

        public bool IsDirectory => false;
        
        private readonly EmbeddedResourceItem _resourceItem;

        public EmbeddedResourceItemFileInfo(EmbeddedResourceItem resourceItem)
        {
            this._resourceItem = resourceItem;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(this._resourceItem.Content);
        }
    }
}