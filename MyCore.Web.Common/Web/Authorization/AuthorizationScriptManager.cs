﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyCoreFramework.Authorization;
using MyCoreFramework.Dependency;
using MyCoreFramework.Runtime.Session;

namespace MyCore.Web.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationScriptManager : IAuthorizationScriptManager, ITransientDependency
    {
        /// <inheritdoc/>
        public IAbpSession AbpSession { get; set; }

        private readonly IPermissionManager _permissionManager;

        public IPermissionChecker PermissionChecker { get; set; }

        /// <inheritdoc/>
        public AuthorizationScriptManager(IPermissionManager permissionManager)
        {
            this.AbpSession = NullAbpSession.Instance;
            this.PermissionChecker = NullPermissionChecker.Instance;

            this._permissionManager = permissionManager;
        }

        /// <inheritdoc/>
        public async Task<string> GetScriptAsync()
        {
            var allPermissionNames = this._permissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (this.AbpSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await this.PermissionChecker.IsGrantedAsync(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }
            
            var script = new StringBuilder();

            script.AppendLine("(function(){");

            script.AppendLine();

            script.AppendLine("    abp.auth = abp.auth || {};");

            script.AppendLine();

            AppendPermissionList(script, "allPermissions", allPermissionNames);

            script.AppendLine();

            AppendPermissionList(script, "grantedPermissions", grantedPermissionNames);

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }

        private static void AppendPermissionList(StringBuilder script, string name, IReadOnlyList<string> permissions)
        {
            script.AppendLine("    abp.auth." + name + " = {");

            for (var i = 0; i < permissions.Count; i++)
            {
                var permission = permissions[i];
                if (i < permissions.Count - 1)
                {
                    script.AppendLine("        '" + permission + "': true,");
                }
                else
                {
                    script.AppendLine("        '" + permission + "': true");
                }
            }

            script.AppendLine("    };");
        }
    }
}
