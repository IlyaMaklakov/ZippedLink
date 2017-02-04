using System;

using AutoMapper;

using MyCoreFramework.Collections.Extensions;

namespace MyCore.AutoMapper
{
    public class AutoMapFromAttribute : AutoMapAttributeBase
    {
        public MemberList MemberList { get; set; } = MemberList.Destination;

        public AutoMapFromAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public AutoMapFromAttribute(MemberList memberList, params Type[] targetTypes)
            : this(targetTypes)
        {
            this.MemberList = memberList;
        }

        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (this.TargetTypes.IsNullOrEmpty())
            {
                return;
            }

            foreach (var targetType in this.TargetTypes)
            {
                configuration.CreateMap(targetType, type, MemberList.Destination);
            }
        }
    }
}