using System;
using System.Globalization;

namespace MyCoreFramework.Localization
{
    /// <summary>
    /// A class that gets the same string on every localization.
    /// </summary>
    [Serializable]
    public class FixedLocalizableString : ILocalizableString
    {
        /// <summary>
        /// The fixed string.
        /// Whenever Localize methods called, this string is returned.
        /// </summary>
        public virtual string FixedString { get; private set; }

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private FixedLocalizableString()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="FixedLocalizableString"/>.
        /// </summary>
        /// <param name="fixedString">
        /// The fixed string.
        /// Whenever Localize methods called, this string is returned.
        /// </param>
        public FixedLocalizableString(string fixedString)
        {
            this.FixedString = fixedString;
        }

        public string Localize(ILocalizationContext context)
        {
            return this.FixedString;
        }

        public string Localize(ILocalizationContext context, CultureInfo culture)
        {
            return this.FixedString;
        }

        public override string ToString()
        {
            return this.FixedString;
        }
    }
}