using System;
using System.IO;
using System.Reflection;

using MyCoreFramework.JetBrains.Annotations;

namespace MyCoreFramework.Resources.Embedded
{
    /// <summary>
    /// Stores needed information of an embedded resource.
    /// </summary>
    public class EmbeddedResourceItem
    {
        /// <summary>
        /// File name including extension.
        /// </summary>
        public string FileName { get; }

        [CanBeNull]
        public string FileExtension { get; }

        /// <summary>
        /// Content of the resource file.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// The assembly that contains the resource.
        /// </summary>
        public Assembly Assembly { get; set; }

        public DateTime LastModifiedUtc { get; }

        internal EmbeddedResourceItem(string fileName, byte[] content, Assembly assembly)
        {
            this.FileName = fileName;
            this.Content = content;
            this.Assembly = assembly;
            this.FileExtension = CalculateFileExtension(this.FileName);
            this.LastModifiedUtc = this.Assembly.Location != null
                ? new FileInfo(this.Assembly.Location).LastWriteTimeUtc
                : DateTime.UtcNow;
        }

        private static string CalculateFileExtension(string fileName)
        {
            if (!fileName.Contains("."))
            {
                return null;
            }

            return fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
        }
    }
}