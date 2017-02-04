namespace MyCoreFramework.Threading
{
    /// <summary>
    /// Base implementation of <see cref="IRunnable"/>.
    /// </summary>
    public abstract class RunnableBase : IRunnable
    {
        /// <summary>
        /// A boolean value to control the running.
        /// </summary>
        public bool IsRunning { get { return this._isRunning; } }

        private volatile bool _isRunning;

        public virtual void Start()
        {
            this._isRunning = true;
        }

        public virtual void Stop()
        {
            this._isRunning = false;
        }

        public virtual void WaitToStop()
        {

        }
    }
}