using System.Globalization;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Extensions;
using MyCoreFramework.Logging;

namespace MyCoreFramework.Localization
{
    public static class LocalizationSourceHelper
    {
        public static string ReturnGivenNameOrThrowException(ILocalizationConfiguration configuration, string sourceName, string name, CultureInfo culture)
        {
            var exceptionMessage = string.Format(
                "Can not find '{0}' in localization source '{1}'!",
                name, sourceName
                );

            if (!configuration.ReturnGivenTextIfNotFound)
            {
                throw new AbpException(exceptionMessage);
            }

            LogHelper.Logger.Warn(exceptionMessage);

            var notFoundText = configuration.HumanizeTextIfNotFound
                ? name.ToSentenceCase(culture)
                : name;

            return configuration.WrapGivenTextIfNotFound
                ? string.Format("[{0}]", notFoundText)
                : notFoundText;
        }
    }
}
