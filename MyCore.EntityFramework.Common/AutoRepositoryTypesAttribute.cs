using System;

namespace MyCore.EntityFramework
{
    /// <summary>
    /// Used to define auto-repository types for entities.
    /// This can be used for DbContext types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRepositoryTypesAttribute : Attribute
    {
        public Type RepositoryInterface { get; private set; }

        public Type RepositoryInterfaceWithPrimaryKey { get; private set; }

        public Type RepositoryImplementation { get; private set; }

        public Type RepositoryImplementationWithPrimaryKey { get; private set; }

        public AutoRepositoryTypesAttribute(
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            this.RepositoryInterface = repositoryInterface;
            this.RepositoryInterfaceWithPrimaryKey = repositoryInterfaceWithPrimaryKey;
            this.RepositoryImplementation = repositoryImplementation;
            this.RepositoryImplementationWithPrimaryKey = repositoryImplementationWithPrimaryKey;
        }
    }
}