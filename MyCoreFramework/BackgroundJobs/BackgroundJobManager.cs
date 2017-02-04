using System;
using System.Threading.Tasks;

using MyCoreFramework.Dependency;
using MyCoreFramework.Events.Bus;
using MyCoreFramework.Events.Bus.Exceptions;
using MyCoreFramework.Json;
using MyCoreFramework.Threading;
using MyCoreFramework.Threading.BackgroundWorkers;
using MyCoreFramework.Threading.Timers;
using MyCoreFramework.Timing;

using Newtonsoft.Json;

namespace MyCoreFramework.BackgroundJobs
{
    /// <summary>
    /// Default implementation of <see cref="IBackgroundJobManager"/>.
    /// </summary>
    public class BackgroundJobManager : PeriodicBackgroundWorkerBase, IBackgroundJobManager, ISingletonDependency
    {
        public IEventBus EventBus { get; set; }
        
        /// <summary>
        /// Interval between polling jobs from <see cref="IBackgroundJobStore"/>.
        /// Default value: 5000 (5 seconds).
        /// </summary>
        public static int JobPollPeriod { get; set; }

        private readonly IIocResolver _iocResolver;
        private readonly IBackgroundJobStore _store;

        static BackgroundJobManager()
        {
            JobPollPeriod = 5000;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobManager"/> class.
        /// </summary>
        public BackgroundJobManager(
            IIocResolver iocResolver,
            IBackgroundJobStore store,
            AbpTimer timer)
            : base(timer)
        {
            this._store = store;
            this._iocResolver = iocResolver;

            this.EventBus = NullEventBus.Instance;

            this.Timer.Period = JobPollPeriod;
        }

        public async Task EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJob<TArgs>
        {
            var jobInfo = new BackgroundJobInfo
            {
                JobType = typeof(TJob).AssemblyQualifiedName,
                JobArgs = args.ToJsonString(),
                Priority = priority
            };

            if (delay.HasValue)
            {
                jobInfo.NextTryTime = Clock.Now.Add(delay.Value);
            }

            await this._store.InsertAsync(jobInfo);
        }

        protected override void DoWork()
        {
            var waitingJobs = AsyncHelper.RunSync(() => this._store.GetWaitingJobsAsync(1000));

            foreach (var job in waitingJobs)
            {
                this.TryProcessJob(job);
            }
        }

        private void TryProcessJob(BackgroundJobInfo jobInfo)
        {
            try
            {
                jobInfo.TryCount++;
                jobInfo.LastTryTime = Clock.Now;

                var jobType = Type.GetType(jobInfo.JobType);
                using (var job = this._iocResolver.ResolveAsDisposable(jobType))
                {
                    try
                    {
                        var jobExecuteMethod = job.Object.GetType().GetMethod("Execute");
                        var argsType = jobExecuteMethod.GetParameters()[0].ParameterType;
                        var argsObj = JsonConvert.DeserializeObject(jobInfo.JobArgs, argsType);

                        jobExecuteMethod.Invoke(job.Object, new[] { argsObj });

                        AsyncHelper.RunSync(() => this._store.DeleteAsync(jobInfo));
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex.Message, ex);

                        var nextTryTime = jobInfo.CalculateNextTryTime();
                        if (nextTryTime.HasValue)
                        {
                            jobInfo.NextTryTime = nextTryTime.Value;
                        }
                        else
                        {
                            jobInfo.IsAbandoned = true;
                        }

                        this.TryUpdate(jobInfo);

                        this.EventBus.Trigger(
                            this,
                            new AbpHandledExceptionData(
                                new BackgroundJobException(
                                    "A background job execution is failed. See inner exception for details. See BackgroundJob property to get information on the background job.", 
                                    ex
                                    )
                                {
                                    BackgroundJob = jobInfo
                                }
                            )
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex.ToString(), ex);

                jobInfo.IsAbandoned = true;

                this.TryUpdate(jobInfo);
            }
        }

        private void TryUpdate(BackgroundJobInfo jobInfo)
        {
            try
            {
                this._store.UpdateAsync(jobInfo);
            }
            catch (Exception updateEx)
            {
                this.Logger.Warn(updateEx.ToString(), updateEx);
            }
        }
    }
}
