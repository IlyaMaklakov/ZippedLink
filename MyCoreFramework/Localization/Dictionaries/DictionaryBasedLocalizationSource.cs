using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;

namespace MyCoreFramework.Localization.Dictionaries
{
    /// <summary>
    /// This class is used to build a localization source
    /// which works on memory based dictionaries to find strings.
    /// </summary>
    public class DictionaryBasedLocalizationSource : IDictionaryBasedLocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; private set; }

        public ILocalizationDictionaryProvider DictionaryProvider { get { return this._dictionaryProvider; } }

        protected ILocalizationConfiguration LocalizationConfiguration { get; private set; }

        private readonly ILocalizationDictionaryProvider _dictionaryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dictionaryProvider"></param>
        public DictionaryBasedLocalizationSource(string name, ILocalizationDictionaryProvider dictionaryProvider)
        {
            if (name.IsNullOrEmpty())
            {
                throw new ArgumentNullException("name");
            }

            this.Name = name;

            if (dictionaryProvider == null)
            {
                throw new ArgumentNullException("dictionaryProvider");
            }

            this._dictionaryProvider = dictionaryProvider;
        }

        /// <inheritdoc/>
        public virtual void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            this.LocalizationConfiguration = configuration;
            this.DictionaryProvider.Initialize(this.Name);
        }

        /// <inheritdoc/>
        public string GetString(string name)
        {
            return this.GetString(name, Thread.CurrentThread.CurrentUICulture);
        }

        /// <inheritdoc/>
        public string GetString(string name, CultureInfo culture)
        {
            var value = this.GetStringOrNull(name, culture);

            if (value == null)
            {
                return this.ReturnGivenNameOrThrowException(name, culture);
            }

            return value;
        }

        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            return this.GetStringOrNull(name, Thread.CurrentThread.CurrentUICulture, tryDefaults);
        }

        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            var cultureName = culture.Name;
            var dictionaries = this.DictionaryProvider.Dictionaries;

            //Try to get from original dictionary (with country code)
            ILocalizationDictionary originalDictionary;
            if (dictionaries.TryGetValue(cultureName, out originalDictionary))
            {
                var strOriginal = originalDictionary.GetOrNull(name);
                if (strOriginal != null)
                {
                    return strOriginal.Value;
                }
            }

            if (!tryDefaults)
            {
                return null;
            }

            //Try to get from same language dictionary (without country code)
            if (cultureName.Contains("-")) //Example: "tr-TR" (length=5)
            {
                ILocalizationDictionary langDictionary;
                if (dictionaries.TryGetValue(GetBaseCultureName(cultureName), out langDictionary))
                {
                    var strLang = langDictionary.GetOrNull(name);
                    if (strLang != null)
                    {
                        return strLang.Value;
                    }
                }
            }

            //Try to get from default language
            var defaultDictionary = this.DictionaryProvider.DefaultDictionary;
            if (defaultDictionary == null)
            {
                return null;
            }

            var strDefault = defaultDictionary.GetOrNull(name);
            if (strDefault == null)
            {
                return null;
            }

            return strDefault.Value;
        }

        /// <inheritdoc/>
        public IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            return this.GetAllStrings(Thread.CurrentThread.CurrentUICulture, includeDefaults);
        }

        /// <inheritdoc/>
        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            //TODO: Can be optimized (example: if it's already default dictionary, skip overriding)

            var dictionaries = this.DictionaryProvider.Dictionaries;

            //Create a temp dictionary to build
            var allStrings = new Dictionary<string, LocalizedString>();

            if (includeDefaults)
            {
                //Fill all strings from default dictionary
                var defaultDictionary = this.DictionaryProvider.DefaultDictionary;
                if (defaultDictionary != null)
                {
                    foreach (var defaultDictString in defaultDictionary.GetAllStrings())
                    {
                        allStrings[defaultDictString.Name] = defaultDictString;
                    }
                }

                //Overwrite all strings from the language based on country culture
                if (culture.Name.Contains("-"))
                {
                    ILocalizationDictionary langDictionary;
                    if (dictionaries.TryGetValue(GetBaseCultureName(culture.Name), out langDictionary))
                    {
                        foreach (var langString in langDictionary.GetAllStrings())
                        {
                            allStrings[langString.Name] = langString;
                        }
                    }
                }
            }

            //Overwrite all strings from the original dictionary
            ILocalizationDictionary originalDictionary;
            if (dictionaries.TryGetValue(culture.Name, out originalDictionary))
            {
                foreach (var originalLangString in originalDictionary.GetAllStrings())
                {
                    allStrings[originalLangString.Name] = originalLangString;
                }
            }

            return allStrings.Values.ToImmutableList();
        }

        /// <summary>
        /// Extends the source with given dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary to extend the source</param>
        public virtual void Extend(ILocalizationDictionary dictionary)
        {
            this.DictionaryProvider.Extend(dictionary);
        }

        protected virtual string ReturnGivenNameOrThrowException(string name, CultureInfo culture)
        {
            return LocalizationSourceHelper.ReturnGivenNameOrThrowException(this.LocalizationConfiguration, this.Name, name, culture);
        }

        private static string GetBaseCultureName(string cultureName)
        {
            return cultureName.Contains("-")
                ? cultureName.Left(cultureName.IndexOf("-", StringComparison.InvariantCulture))
                : cultureName;
        }
    }
}
