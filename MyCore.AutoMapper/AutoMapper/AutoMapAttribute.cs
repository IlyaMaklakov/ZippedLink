using System;

using AutoMapper;

using MyCoreFramework.Collections.Extensions;

namespace MyCore.AutoMapper
{
    public class AutoMapAttribute : AutoMapAttributeBase
    {
        public AutoMapAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (this.TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (var targetType in this.TargetTypes)
            {
                configuration.CreateMap(type, targetType, MemberList.Source);
                configuration.CreateMap(targetType, type, MemberList.Destination);
            }
        }
    }
}