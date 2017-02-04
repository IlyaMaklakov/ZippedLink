using System;

using MyCore.Web.Configuration;

using MyCoreFramework.Dependency;
using MyCoreFramework.Localization;

namespace MyCore.Web.Models
{
    /// <inheritdoc/>
    public class ErrorInfoBuilder : IErrorInfoBuilder, ISingletonDependency
    {
        private IExceptionToErrorInfoConverter Converter { get; set; }

        /// <inheritdoc/>
        public ErrorInfoBuilder(IAbpWebCommonModuleConfiguration configuration, ILocalizationManager localizationManager)
        {
            this.Converter = new DefaultErrorInfoConverter(configuration, localizationManager);
        }

        /// <inheritdoc/>
        public ErrorInfo BuildForException(Exception exception)
        {
            return this.Converter.Convert(exception);
        }

        /// <summary>
        /// Adds an exception converter that is used by <see cref="BuildForException"/> method.
        /// </summary>
        /// <param name="converter">Converter object</param>
        public void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = this.Converter;
            this.Converter = converter;
        }
    }
}
