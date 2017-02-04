using System;
using System.Collections.Generic;
using System.Linq;

using MyCoreFramework;
using MyCoreFramework.Collections.Extensions;
using MyCoreFramework.Dependency;
using MyCoreFramework.Domain.Uow;
using MyCoreFramework.MultiTenancy;

namespace MyCore.EntityFramework
{
    public abstract class DbContextTypeMatcher<TBaseDbContext> : IDbContextTypeMatcher, ISingletonDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly Dictionary<Type, List<Type>> _dbContextTypes;

        protected DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            this._currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            this._dbContextTypes = new Dictionary<Type, List<Type>>();
        }

        public void Populate(Type[] dbContextTypes)
        {
            foreach (var dbContextType in dbContextTypes)
            {
                var types = new List<Type>();

                AddWithBaseTypes(dbContextType, types);

                foreach (var type in types)
                {
                    this.Add(type, dbContextType);
                }
            }
        }

        //TODO: GetConcreteType method can be optimized by extracting/caching MultiTenancySideAttribute attributes for DbContexes.

        public virtual Type GetConcreteType(Type sourceDbContextType)
        {
            //TODO: This can also get MultiTenancySide to filter dbcontexes

            if (!sourceDbContextType.IsAbstract)
            {
                return sourceDbContextType;
            }
            
            //Get possible concrete types for given DbContext type
            var allTargetTypes = this._dbContextTypes.GetOrDefault(sourceDbContextType);

            if (allTargetTypes.IsNullOrEmpty())
            {
                throw new AbpException("Could not find a concrete implementation of given DbContext type: " + sourceDbContextType.AssemblyQualifiedName);
            }

            if (allTargetTypes.Count == 1)
            {
                //Only one type does exists, return it
                return allTargetTypes[0];
            }

            this.CheckCurrentUow();

            var currentTenancySide = this.GetCurrentTenancySide();

            var multiTenancySideContexes = GetMultiTenancySideContextTypes(allTargetTypes, currentTenancySide);

            if (multiTenancySideContexes.Count == 1)
            {
                return multiTenancySideContexes[0];
            }

            if (multiTenancySideContexes.Count > 1)
            {
                return GetDefaultDbContextType(multiTenancySideContexes, sourceDbContextType, currentTenancySide);
            }

            return GetDefaultDbContextType(allTargetTypes, sourceDbContextType, currentTenancySide);
        }

        private void CheckCurrentUow()
        {
            if (this._currentUnitOfWorkProvider.Current == null)
            {
                throw new AbpException("GetConcreteType method should be called in a UOW.");
            }
        }

        private MultiTenancySides GetCurrentTenancySide()
        {
            return this._currentUnitOfWorkProvider.Current.GetTenantId() == null
                       ? MultiTenancySides.Host
                       : MultiTenancySides.Tenant;
        }

        private static List<Type> GetMultiTenancySideContextTypes(List<Type> dbContextTypes, MultiTenancySides tenancySide)
        {
            return dbContextTypes.Where(type =>
            {
                var attrs = type.GetCustomAttributes(typeof(MultiTenancySideAttribute), true);
                if (attrs.IsNullOrEmpty())
                {
                    return false;
                }

                return ((MultiTenancySideAttribute)attrs[0]).Side.HasFlag(tenancySide);
            }).ToList();
        }

        private static Type GetDefaultDbContextType(List<Type> dbContextTypes, Type sourceDbContextType, MultiTenancySides tenancySide)
        {
            var filteredTypes = dbContextTypes
                .Where(type => !type.IsDefined(typeof(AutoRepositoryTypesAttribute), true))
                .ToList();

            if (filteredTypes.Count == 1)
            {
                return filteredTypes[0];
            }

            filteredTypes = filteredTypes
                .Where(type => !type.IsDefined(typeof(DefaultDbContextAttribute), true))
                .ToList();

            if (filteredTypes.Count == 1)
            {
                return filteredTypes[0];
            }

            throw new AbpException(string.Format(
                "Found more than one concrete type for given DbContext Type ({0}) define MultiTenancySideAttribute with {1}. Found types: {2}.",
                sourceDbContextType,
                tenancySide,
                dbContextTypes.Select(c => c.AssemblyQualifiedName).JoinAsString(", ")
                ));
        }

        private static void AddWithBaseTypes(Type dbContextType, List<Type> types)
        {
            types.Add(dbContextType);
            if (dbContextType != typeof(TBaseDbContext))
            {
                AddWithBaseTypes(dbContextType.BaseType, types);
            }
        }

        private void Add(Type sourceDbContextType, Type targetDbContextType)
        {
            if (!this._dbContextTypes.ContainsKey(sourceDbContextType))
            {
                this._dbContextTypes[sourceDbContextType] = new List<Type>();
            }

            this._dbContextTypes[sourceDbContextType].Add(targetDbContextType);
        }
    }
}