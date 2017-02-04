using AutoMapper;

using MyCoreFramework.Dependency;

using IObjectMapper = MyCoreFramework.ObjectMapping.IObjectMapper;

namespace MyCore.AutoMapper
{
    public class AutoMapperObjectMapper : IObjectMapper, ISingletonDependency
    {
        private readonly IMapper _mapper;

        public AutoMapperObjectMapper(IMapper mapper)
        {
            this._mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return this._mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return this._mapper.Map(source, destination);
        }
    }
}
