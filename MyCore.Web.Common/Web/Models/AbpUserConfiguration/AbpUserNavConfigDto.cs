using System.Collections.Generic;

using MyCoreFramework.Application.Navigation;

namespace MyCore.Web.Models.AbpUserConfiguration
{
    public class AbpUserNavConfigDto
    {
        public Dictionary<string, UserMenu> Menus { get; set; }
    }
}