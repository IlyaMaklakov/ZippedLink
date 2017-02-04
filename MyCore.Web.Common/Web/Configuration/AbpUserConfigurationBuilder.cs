using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MyCore.Web.Models.AbpUserConfiguration;
using MyCore.Web.Security.AntiForgery;

using MyCoreFramework.Application.Features;
using MyCoreFramework.Application.Navigation;
using MyCoreFramework.Authorization;
using MyCoreFramework.Configuration;
using MyCoreFramework.Configuration.Startup;
using MyCoreFramework.Dependency;
using MyCoreFramework.Extensions;
using MyCoreFramework.Localization;
using MyCoreFramework.Runtime.Session;
using MyCoreFramework.Timing;
using MyCoreFramework.Timing.Timezone;

namespace MyCore.Web.Configuration
{
    public class AbpUserConfigurationBuilder : ITransientDependency
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ILanguageManager _languageManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IPermissionManager _permissionManager;
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;
        private readonly IAbpAntiForgeryConfiguration _abpAntiForgeryConfiguration;
        private readonly IAbpSession _abpSession;
        private readonly IPermissionChecker _permissionChecker;

        public AbpUserConfigurationBuilder(
            IMultiTenancyConfig multiTenancyConfig,
            ILanguageManager languageManager,
            ILocalizationManager localizationManager,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker,
            IPermissionManager permissionManager,
            IUserNavigationManager userNavigationManager,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            IAbpAntiForgeryConfiguration abpAntiForgeryConfiguration,
            IAbpSession abpSession,
            IPermissionChecker permissionChecker)
        {
            this._multiTenancyConfig = multiTenancyConfig;
            this._languageManager = languageManager;
            this._localizationManager = localizationManager;
            this._featureManager = featureManager;
            this._featureChecker = featureChecker;
            this._permissionManager = permissionManager;
            this._userNavigationManager = userNavigationManager;
            this._settingDefinitionManager = settingDefinitionManager;
            this._settingManager = settingManager;
            this._abpAntiForgeryConfiguration = abpAntiForgeryConfiguration;
            this._abpSession = abpSession;
            this._permissionChecker = permissionChecker;
        }

        public async Task<AbpUserConfigurationDto> GetAll()
        {
            return new AbpUserConfigurationDto
            {
                MultiTenancy = this.GetUserMultiTenancyConfig(),
                Session = this.GetUserSessionConfig(),
                Localization = this.GetUserLocalizationConfig(),
                Features = await this.GetUserFeaturesConfig(),
                Auth = await this.GetUserAuthConfig(),
                Nav = await this.GetUserNavConfig(),
                Setting = await this.GetUserSettingConfig(),
                Clock = this.GetUserClockConfig(),
                Timing = await this.GetUserTimingConfig(),
                Security = this.GetUserSecurityConfig()
            };
        }

        private AbpMultiTenancyConfigDto GetUserMultiTenancyConfig()
        {
            return new AbpMultiTenancyConfigDto
            {
                IsEnabled = this._multiTenancyConfig.IsEnabled
            };
        }

        private AbpUserSessionConfigDto GetUserSessionConfig()
        {
            return new AbpUserSessionConfigDto
            {
                UserId = this._abpSession.UserId,
                TenantId = this._abpSession.TenantId,
                ImpersonatorUserId = this._abpSession.ImpersonatorUserId,
                ImpersonatorTenantId = this._abpSession.ImpersonatorTenantId,
                MultiTenancySide = this._abpSession.MultiTenancySide
            };
        }

        private AbpUserLocalizationConfigDto GetUserLocalizationConfig()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
            var languages = this._languageManager.GetLanguages();

            var config = new AbpUserLocalizationConfigDto
            {
                CurrentCulture = new AbpUserCurrentCultureConfigDto
                {
                    Name = currentCulture.Name,
                    DisplayName = currentCulture.DisplayName
                },
                Languages = languages.ToList()
            };

            if (languages.Count > 0)
            {
                config.CurrentLanguage = this._languageManager.CurrentLanguage;
            }

            var sources = this._localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
            config.Sources = sources.Select(s => new AbpLocalizationSourceDto
            {
                Name = s.Name,
                Type = s.GetType().Name
            }).ToList();

            config.Values = new Dictionary<string, Dictionary<string, string>>();
            foreach (var source in sources)
            {
                var stringValues = source.GetAllStrings(currentCulture).OrderBy(s => s.Name).ToList();
                var stringDictionary = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value);
                config.Values.Add(source.Name, stringDictionary);
            }

            return config;
        }

        private async Task<AbpUserFeatureConfigDto> GetUserFeaturesConfig()
        {
            var config = new AbpUserFeatureConfigDto()
            {
                AllFeatures = new Dictionary<string, AbpStringValueDto>()
            };

            var allFeatures = this._featureManager.GetAll().ToList();

            if (this._abpSession.TenantId.HasValue)
            {
                var currentTenantId = this._abpSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    var value = await this._featureChecker.GetValueAsync(currentTenantId, feature.Name);
                    config.AllFeatures.Add(feature.Name, new AbpStringValueDto
                    {
                        Value = value
                    });
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    config.AllFeatures.Add(feature.Name, new AbpStringValueDto
                    {
                        Value = feature.DefaultValue
                    });
                }
            }

            return config;
        }

        private async Task<AbpUserAuthConfigDto> GetUserAuthConfig()
        {
            var config = new AbpUserAuthConfigDto();

            var allPermissionNames = this._permissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (this._abpSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await this._permissionChecker.IsGrantedAsync(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }

            config.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
            config.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");

            return config;
        }

        private async Task<AbpUserNavConfigDto> GetUserNavConfig()
        {
            var userMenus = await this._userNavigationManager.GetMenusAsync(this._abpSession.ToUserIdentifier());
            return new AbpUserNavConfigDto
            {
                Menus = userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu)
            };
        }

        private async Task<AbpUserSettingConfigDto> GetUserSettingConfig()
        {
            var config = new AbpUserSettingConfigDto
            {
                Values = new Dictionary<string, string>()
            };

            var settingDefinitions = this._settingDefinitionManager
                .GetAllSettingDefinitions()
                .Where(sd => sd.IsVisibleToClients);

            foreach (var settingDefinition in settingDefinitions)
            {
                var settingValue = await this._settingManager.GetSettingValueAsync(settingDefinition.Name);
                config.Values.Add(settingDefinition.Name, settingValue);
            }

            return config;
        }

        private AbpUserClockConfigDto GetUserClockConfig()
        {
            return new AbpUserClockConfigDto
            {
                Provider = Clock.Provider.GetType().Name.ToCamelCase()
            };
        }

        private async Task<AbpUserTimingConfigDto> GetUserTimingConfig()
        {
            var timezoneId = await this._settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

            return new AbpUserTimingConfigDto
            {
                TimeZoneInfo = new AbpUserTimeZoneConfigDto
                {
                    Windows = new AbpUserWindowsTimeZoneConfigDto
                    {
                        TimeZoneId = timezoneId,
                        BaseUtcOffsetInMilliseconds = timezone.BaseUtcOffset.TotalMilliseconds,
                        CurrentUtcOffsetInMilliseconds = timezone.GetUtcOffset(Clock.Now).TotalMilliseconds,
                        IsDaylightSavingTimeNow = timezone.IsDaylightSavingTime(Clock.Now)
                    },
                    Iana = new AbpUserIanaTimeZoneConfigDto
                    {
                        TimeZoneId = TimezoneHelper.WindowsToIana(timezoneId)
                    }
                }
            };
        }

        private AbpUserSecurityConfigDto GetUserSecurityConfig()
        {
            return new AbpUserSecurityConfigDto()
            {
                AntiForgery = new AbpUserAntiForgeryConfigDto
                {
                    TokenCookieName = this._abpAntiForgeryConfiguration.TokenCookieName,
                    TokenHeaderName = this._abpAntiForgeryConfiguration.TokenHeaderName
                }
            };
        }
    }
}