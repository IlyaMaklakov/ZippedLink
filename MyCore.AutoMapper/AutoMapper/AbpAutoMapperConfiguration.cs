using System;
using System.Collections.Generic;

using AutoMapper;

namespace MyCore.AutoMapper
{
    public class AbpAutoMapperConfiguration : IAbpAutoMapperConfiguration
    {
        public List<Action<IMapperConfigurationExpression>> Configurators { get; }

        public AbpAutoMapperConfiguration()
        {
            this.Configurators = new List<Action<IMapperConfigurationExpression>>();
        }
    }
}