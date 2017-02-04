using System;
using System.Collections.Generic;

using AutoMapper;

namespace MyCore.AutoMapper
{
    public interface IAbpAutoMapperConfiguration
    {
        List<Action<IMapperConfigurationExpression>> Configurators { get; }
    }
}