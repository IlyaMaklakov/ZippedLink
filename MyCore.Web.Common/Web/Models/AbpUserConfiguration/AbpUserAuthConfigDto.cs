using System.Collections.Generic;

namespace MyCore.Web.Models.AbpUserConfiguration
{
    public class AbpUserAuthConfigDto
    {
        public Dictionary<string,string> AllPermissions { get; set; }

        public Dictionary<string, string> GrantedPermissions { get; set; }
        
    }
}