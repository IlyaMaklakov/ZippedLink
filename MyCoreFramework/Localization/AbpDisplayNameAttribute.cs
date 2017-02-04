using System.ComponentModel;

namespace MyCoreFramework.Localization
{
    public class AbpDisplayNameAttribute : DisplayNameAttribute
    {
        public override string DisplayName => LocalizationHelper.GetString(this.SourceName, this.Key);

        public string SourceName { get; set; }
        public string Key { get; set; }

        public AbpDisplayNameAttribute(string sourceName, string key)
        {
            this.SourceName = sourceName;
            this.Key = key;
        }
    }
}
