using System.Collections.Generic;

namespace MyCore.Web.Models.AbpUserConfiguration
{
    public class AbpUserFeatureConfigDto
    {
        public Dictionary<string, AbpStringValueDto> AllFeatures { get; set; }
    }
}