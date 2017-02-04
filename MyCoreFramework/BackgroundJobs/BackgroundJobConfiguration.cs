using MyCoreFramework.Configuration.Startup;

namespace MyCoreFramework.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public bool IsJobExecutionEnabled { get; set; }
        
        public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        public BackgroundJobConfiguration(IAbpStartupConfiguration abpConfiguration)
        {
            this.AbpConfiguration = abpConfiguration;

            this.IsJobExecutionEnabled = true;
        }
    }
}