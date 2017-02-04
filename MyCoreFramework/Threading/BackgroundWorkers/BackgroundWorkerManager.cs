using System;
using System.Collections.Generic;

using MyCoreFramework.Dependency;

namespace MyCoreFramework.Threading.BackgroundWorkers
{
    /// <summary>
    /// Implements <see cref="IBackgroundWorkerManager"/>.
    /// </summary>
    public class BackgroundWorkerManager : RunnableBase, IBackgroundWorkerManager, ISingletonDependency, IDisposable
    {
        private readonly IIocResolver _iocResolver;
        private readonly List<IBackgroundWorker> _backgroundJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorkerManager"/> class.
        /// </summary>
        public BackgroundWorkerManager(IIocResolver iocResolver)
        {
            this._iocResolver = iocResolver;
            this._backgroundJobs = new List<IBackgroundWorker>();
        }

        public override void Start()
        {
            base.Start();

            this._backgroundJobs.ForEach(job => job.Start());
        }

        public override void Stop()
        {
            this._backgroundJobs.ForEach(job => job.Stop());

            base.Stop();
        }

        public override void WaitToStop()
        {
            this._backgroundJobs.ForEach(job => job.WaitToStop());

            base.WaitToStop();
        }

        public void Add(IBackgroundWorker worker)
        {
            this._backgroundJobs.Add(worker);

            if (this.IsRunning)
            {
                worker.Start();
            }
        }

        private bool _isDisposed;

        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            this._isDisposed = true;

            this._backgroundJobs.ForEach(this._iocResolver.Release);
            this._backgroundJobs.Clear();
        }
    }
}
