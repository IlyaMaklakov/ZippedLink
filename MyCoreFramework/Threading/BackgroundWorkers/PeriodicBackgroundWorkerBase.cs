using System;

using MyCoreFramework.Threading.Timers;

namespace MyCoreFramework.Threading.BackgroundWorkers
{
    /// <summary>
    /// Extends <see cref="BackgroundWorkerBase"/> to add a periodic running Timer. 
    /// </summary>
    public abstract class PeriodicBackgroundWorkerBase : BackgroundWorkerBase
    {
        protected readonly AbpTimer Timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicBackgroundWorkerBase"/> class.
        /// </summary>
        /// <param name="timer">A timer.</param>
        protected PeriodicBackgroundWorkerBase(AbpTimer timer)
        {
            this.Timer = timer;
            this.Timer.Elapsed += this.Timer_Elapsed;
        }

        public override void Start()
        {
            base.Start();
            this.Timer.Start();
        }

        public override void Stop()
        {
            this.Timer.Stop();
            base.Stop();
        }

        public override void WaitToStop()
        {
            this.Timer.WaitToStop();
            base.WaitToStop();
        }

        /// <summary>
        /// Handles the Elapsed event of the Timer.
        /// </summary>
        private void Timer_Elapsed(object sender, System.EventArgs e)
        {
            try
            {
                this.DoWork();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Periodic works should be done by implementing this method.
        /// </summary>
        protected abstract void DoWork();
    }
}