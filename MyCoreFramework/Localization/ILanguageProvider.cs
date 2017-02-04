using System.Collections.Generic;

namespace MyCoreFramework.Localization
{
    public interface ILanguageProvider
    {
        IReadOnlyList<LanguageInfo> GetLanguages();
    }
}