using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Extensions;
using MyCoreFramework.IO.Extensions;

namespace MyCoreFramework.Resources.Embedded
{
    public class EmbeddedResourceSet
    {
        public string RootPath { get; }

        public Assembly Assembly { get; }

        public string ResourceNamespace { get; }

        public EmbeddedResourceSet(string rootPath, Assembly assembly, string resourceNamespace)
        {
            this.RootPath = rootPath.EnsureEndsWith('/');
            this.Assembly = assembly;
            this.ResourceNamespace = resourceNamespace;
        }

        internal void AddResources(Dictionary<string, EmbeddedResourceItem> resources)
        {
            foreach (var resourceName in this.Assembly.GetManifestResourceNames())
            {
                if (!resourceName.StartsWith(this.ResourceNamespace))
                {
                    continue;
                }

                using (var stream = this.Assembly.GetManifestResourceStream(resourceName))
                {
                    var filePath = this.RootPath + this.ConvertToRelativePath(resourceName);

                    resources[filePath] = new EmbeddedResourceItem(
                        CalculateFileName(filePath),
                        stream.GetAllBytes(),
                        this.Assembly
                    );
                }
            }
        }

        private string ConvertToRelativePath(string resourceName)
        {
            resourceName = resourceName.Substring(this.ResourceNamespace.Length + 1);

            var pathParts = resourceName.Split('.');
            if (pathParts.Length <= 2)
            {
                return resourceName;
            }

            var folder = pathParts.Take(pathParts.Length - 2).Select(NormalizeFolderName).JoinAsString("/");
            var fileName = pathParts[pathParts.Length - 2] + "." + pathParts[pathParts.Length - 1];

            return folder + "/" + fileName;
        }

        private static string NormalizeFolderName(string pathPart)
        {
            //TODO: Implement all rules of .NET
            return pathPart.Replace('-', '_');
        }

        private static string CalculateFileName(string filePath)
        {
            if (!filePath.Contains("/"))
            {
                return filePath;
            }

            return filePath.Substring(filePath.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }
    }
}