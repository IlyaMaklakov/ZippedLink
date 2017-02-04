using System;

using AutoMapper;

using MyCoreFramework.Collections.Extensions;

namespace MyCore.AutoMapper
{
    public class AutoMapToAttribute : AutoMapAttributeBase
    {
        public MemberList MemberList { get; set; } = MemberList.Source;

        public AutoMapToAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }

        public AutoMapToAttribute(MemberList memberList, params Type[] targetTypes)
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
                configuration.CreateMap(type, targetType, this.MemberList);
            }
        }
    }
}